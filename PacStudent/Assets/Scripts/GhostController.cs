using System.Collections;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GhostController : MonoBehaviour
{
    public bool isScared;
    public bool isRecovering;
    public bool isNormal;
    public bool isDead;
    private bool isRespawning;
    public int ghostID;
    private GameObject pacStudent;
    private GameObject gameManager;
    private ScoreKeeper scoreKeeper;
    private HUDManager hudManager;
    private bool isInSpawn;

    [Header("Pathfinding")]
    public Tilemap groundTilemap;
    public Tilemap ghostNormalTilemap;
    public float moveSpeed = 5.0f;
    private float moveTime;
    private bool isLerping;
    private Vector2 lastInput;
    private Vector2 currentInput;
    private Vector3 startPosition;
    private Vector3 endPosition;
    private Vector3 spawnPosition;
    public GameObject northWayPoint;
    public GameObject southWayPoint;
    private GameObject wayPoint;
    private GameObject topLeft;
    private GameObject topRight;
    private GameObject bottomLeft;
    private GameObject bottomRight;
    private GameObject mostRecentWayPoint;
    private GameObject middleBottomRight;
    private GameObject middleBottomLeft;
    private GameObject middleTopRight;
    private GameObject middleTopLeft;
    private GameObject middleRight;
    private bool isHunting;
    private bool Level2;
    private DifficultyManager difficultyManager;


    // Start is called before the first frame update
    void Start()
    {
        // Waypoints for Ghost4
        topLeft = GameObject.FindGameObjectWithTag("TopLeft");
        mostRecentWayPoint = topRight = GameObject.FindGameObjectWithTag("TopRight");
        bottomLeft = GameObject.FindGameObjectWithTag("BottomLeft");
        bottomRight = GameObject.FindGameObjectWithTag("BottomRight");
        middleBottomRight = GameObject.FindGameObjectWithTag("MiddleBottomRight");
        middleBottomLeft = GameObject.FindGameObjectWithTag("MiddleBottomLeft");
        middleTopRight = GameObject.FindGameObjectWithTag("MiddleTopRight");
        middleTopLeft = GameObject.FindGameObjectWithTag("MiddleTopLeft");
        middleRight = GameObject.FindGameObjectWithTag("MiddleRight");

        gameManager = GameObject.FindGameObjectWithTag("GameController");
        pacStudent = GameObject.FindGameObjectWithTag("Player");
        scoreKeeper = gameManager.GetComponent<ScoreKeeper>();
        hudManager = gameManager.GetComponent<HUDManager>();
        Level2 = hudManager.Level2;
        currentInput = lastInput = Vector2.right;
        startPosition = endPosition = spawnPosition = transform.position;
        isLerping = false;
        difficultyManager = gameManager.GetComponent<DifficultyManager>();
        if (ghostID == 1 || ghostID == 4)
        {
            wayPoint = northWayPoint;
        }
        else
        {
            wayPoint = southWayPoint;
        }
        NormalState();
        if (Level2){
            StartCoroutine(HuntTimer());
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (GetComponent<Animator>().GetBool("ScaredAnim") != isScared)
        {
            GetComponent<Animator>().SetBool("ScaredAnim", isScared);
        }
        if (GetComponent<Animator>().GetBool("RecoveryAnim") != isRecovering)
        {
            GetComponent<Animator>().SetBool("RecoveryAnim", isRecovering);
        }
        if (GetComponent<Animator>().GetBool("NormalAnim") != isNormal)
        {
            GetComponent<Animator>().SetBool("NormalAnim", isNormal);
        }
        if (GetComponent<Animator>().GetBool("DeadAnim") != isDead)
        {
            GetComponent<Animator>().SetBool("DeadAnim", isDead);
        }

        if (hudManager.gameStarted && !isDead)
        {
            if (!isLerping)
            {
                GatherInput();
                TryMove(lastInput);
            }

            if (isLerping)
            {
                ContinueLerp();
            }
        }
        else if (isDead && !isRespawning)
        {
            StartCoroutine(DeadGhostPath());
        }
    }

    public void ScaredState()
    {
        isScared = true;
        isRecovering = false;
        isNormal = false;
        isDead = false;

        //Debug.Log(this.gameObject.name + " is scared!");
    }

    public void RecoveryState()
    {
        isRecovering = true;
        isScared = false;
        isNormal = false;
        isDead = false;

        //Debug.Log(this.gameObject.name + " is recovering!");
    }
    public void NormalState()
    {
        if (!isDead)
        {
            isNormal = true;
            isScared = false;
            isRecovering = false;
            isDead = false;

            //Debug.Log(this.gameObject.name + " is normal!");
        }
    }

    public void DeadState()
    {
        isDead = true;
        isNormal = false;
        isScared = false;
        isRecovering = false;

        //Debug.Log(this.gameObject.name + " is dead!");
        scoreKeeper.AddScore(300);

        StartCoroutine(DeadTimer());
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (isNormal) { NormalCollison(); }
            if (isScared) { ScaredCollision(); }
            if (isRecovering) { RecoveringCollision(); }
        }
        else if (other.gameObject.tag == "Spawn")
        {
            isInSpawn = true;
            //Debug.Log("Ghost is in spawn");
        }
        else if (other.gameObject.tag != "Enemy" || other.gameObject.tag != "Cherry" || other.gameObject.tag != "PowerPellet")
        {
            mostRecentWayPoint = other.gameObject;
        }

    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "Spawn")
        {
            isInSpawn = false;
            //Debug.Log("Ghost is out of spawn");
        }
    }

    private void NormalCollison()
    {
        pacStudent.GetComponent<PacStudentController>().KillPlayer();
        //Debug.Log("Normal collision..");
    }

    private void ScaredCollision()
    {
        DeadState();
        //Debug.Log("Scared collision..");
    }

    private void RecoveringCollision()
    {
        DeadState();
        //Debug.Log("Recovering collision..");
    }

    private IEnumerator DeadTimer()
    {
        yield return new WaitForSeconds(5.0f);
        isDead = false;
        mostRecentWayPoint = topRight;
        NormalState();
    }

    private void TryMove(Vector2 direction)
    {
        /*
        if (currentInput == -lastInput){
            return;
        }
        */
        // get the next cell
        Vector3Int nextCell = groundTilemap.WorldToCell(transform.position + new Vector3(direction.x, direction.y, 0));

        if (IsWalkable(nextCell)) // only move if the cell is walkable
        {

            currentInput = direction;
            StartLerp(GetCellCenterWorld(nextCell));
        }
        else if (currentInput != Vector2.zero)
        {
            nextCell = groundTilemap.WorldToCell(transform.position + new Vector3(currentInput.x, currentInput.y, 0));
            if (IsWalkable(nextCell))
            {

                StartLerp(GetCellCenterWorld(nextCell));
            }
            else // This condition will only be called when hitting a wall
            {
                currentInput = Vector2.zero;
            }
        }
    }

    private void StartLerp(Vector3 worldPosition) // begin the lerp
    {
        startPosition = transform.position;
        endPosition = worldPosition;
        moveTime = 0f;
        isLerping = true;
    }

    private void ContinueLerp() //continues lerp
    {
        moveTime += Time.deltaTime * moveSpeed;
        transform.position = Vector3.Lerp(startPosition, endPosition, moveTime);

        if (Vector3.Distance(transform.position, endPosition) < 0.01f)
        {
            transform.position = endPosition;
            isLerping = false;
        }
    }
    private void GatherInput() // decide which ai is needed
    {
        if (isNormal && !isInSpawn)
        {
            if (isHunting)
            {
                HuntingPath();
            }
            else
            {
                switch (ghostID)
                {
                    case 1:
                        Ghost1Path();
                        break;
                    case 2:
                        Ghost2Path();
                        break;
                    case 3:
                        Ghost3Path();
                        break;
                    case 4:
                        Ghost4Path();
                        break;
                }
            }
        }
        else if (isInSpawn && !isDead)
        {
            leaveBoxPath();
        }
        else
        {
            Ghost1Path();
        }
    }
    private Vector3 GetCellCenterWorld(Vector3Int cellPosition) // i had to add this cause other wise he goes to the 0,0 of the cell which is a corner
    {
        return groundTilemap.GetCellCenterWorld(cellPosition);
    }

    private bool IsWalkable(Vector3Int cellPosition) // return true if the cell is walkable
    {
        if (isInSpawn)
        {
            return groundTilemap.HasTile(cellPosition);
        }
        else
        {
            return ghostNormalTilemap.HasTile(cellPosition);
        }

    }

    private void Ghost1Path()
    {
        float distanceFromPlayer = Vector3.Distance(transform.position, pacStudent.transform.position);

        Vector2[] possibleDirections = new Vector2[] {
            Vector2.up,
            Vector2.down,
            Vector2.left,
            Vector2.right
        };

        List<Vector2> validDirections = new List<Vector2>(possibleDirections);

        Vector2 oppositeLastInput = -lastInput;
        validDirections.Remove(oppositeLastInput);

        // filter derections that would birng ghost closer to player
        List<Vector2> furtherDirections = validDirections.Where(dir => Vector3.Distance(transform.position + new Vector3(dir.x, dir.y, 0), pacStudent.transform.position) >= distanceFromPlayer).ToList();

        Vector2 direction;
        // go to the furthest direction and if there isnt just pick a random one
        if (furtherDirections.Count > 0)
        {

            direction = furtherDirections[Random.Range(0, furtherDirections.Count)];
        }
        else
        {
            direction = validDirections[Random.Range(0, validDirections.Count)];
        }

        // this just checks if its walkable
        if (!IsWalkable(groundTilemap.WorldToCell(transform.position + new Vector3(direction.x, direction.y, 0))))
        {
            // pick a random direction if the one we picked isnt gonna worjk
            List<Vector2> walkableDirections = validDirections.Where(dir => IsWalkable(groundTilemap.WorldToCell(transform.position + new Vector3(dir.x, dir.y, 0)))).ToList();

            if (walkableDirections.Count > 0)
            {
                direction = walkableDirections[Random.Range(0, walkableDirections.Count)];
            }
            // there should probably be a else here in case its completely stuck but with this map its not really needed
        }

        lastInput = direction;

    }
    private void Ghost2Path()
    {
        float distanceFromPlayer = Vector3.Distance(transform.position, pacStudent.transform.position);

        Vector2[] possibleDirections = new Vector2[] {
            Vector2.up,
            Vector2.down,
            Vector2.left,
            Vector2.right
        };

        List<Vector2> validDirections = new List<Vector2>(possibleDirections);

        Vector2 oppositeLastInput = -lastInput;
        validDirections.Remove(oppositeLastInput);

        // filter derections that would birng ghost further from the player
        List<Vector2> closestDirections = validDirections.Where(dir => Vector3.Distance(transform.position + new Vector3(dir.x, dir.y, 0), pacStudent.transform.position) <= distanceFromPlayer).ToList();

        Vector2 direction;
        // go to the closest direction and if there isnt just pick a random one
        if (closestDirections.Count > 0)
        {

            direction = closestDirections[Random.Range(0, closestDirections.Count)];
        }
        else
        {
            direction = validDirections[Random.Range(0, validDirections.Count)];
        }

        // this just checks if its walkable
        if (!IsWalkable(groundTilemap.WorldToCell(transform.position + new Vector3(direction.x, direction.y, 0))))
        {
            // pick a random direction if the one we picked isnt gonna worjk
            List<Vector2> walkableDirections = validDirections.Where(dir => IsWalkable(groundTilemap.WorldToCell(transform.position + new Vector3(dir.x, dir.y, 0)))).ToList();

            if (walkableDirections.Count > 0)
            {
                direction = walkableDirections[Random.Range(0, walkableDirections.Count)];
            }
            // there should probably be a else here in case its completely stuck but with this map its not really needed
        }

        lastInput = direction;
    }
    private void Ghost3Path()
    {
        Vector2[] possibleDirections = new Vector2[] {
            Vector2.up,
            Vector2.down,
            Vector2.left,
            Vector2.right
        };

        List<Vector2> validDirections = new List<Vector2>(possibleDirections);

        Vector2 oppositeLastInput = -lastInput;
        validDirections.Remove(oppositeLastInput);

        Vector2 direction = Vector2.zero;
        do
        {
            direction = validDirections[Random.Range(0, validDirections.Count)];
        } while (!IsWalkable(groundTilemap.WorldToCell(transform.position + new Vector3(direction.x, direction.y, 0))));
        //Debug.Log("The last input was " + lastInput + " the opposite is " + oppositeLastInput + " and the new direction is " + direction);
        lastInput = direction;
    }


    private void Ghost4Path()
    {
        float distanceFromWayPoint = Vector3.Distance(transform.position, GetNextWayPoint().transform.position);

        Vector2[] possibleDirections = new Vector2[] {
            Vector2.up,
            Vector2.down,
            Vector2.left,
            Vector2.right
        };

        List<Vector2> validDirections = new List<Vector2>(possibleDirections);

        Vector2 oppositeLastInput = -lastInput;
        validDirections.Remove(oppositeLastInput);

        // sort directions by distance to the waypoint
        List<Vector2> sortedDirections = validDirections.OrderBy(dir => Vector3.Distance(transform.position + new Vector3(dir.x, dir.y, 0), GetNextWayPoint().transform.position)).ToList();

        Vector2 direction = Vector2.zero;
        bool foundPath = false;

        // find the first walkable one
        foreach (var dir in sortedDirections)
        {
            if (IsWalkable(groundTilemap.WorldToCell(transform.position + new Vector3(dir.x, dir.y, 0))))
            {
                direction = dir;
                foundPath = true;
                break;
            }
        }

        // if there isnt a shortest one just pick a random one
        if (!foundPath)
        {
            var walkableDirections = validDirections.Where(dir => IsWalkable(groundTilemap.WorldToCell(transform.position + new Vector3(dir.x, dir.y, 0)))).ToList();

            if (walkableDirections.Count > 0)
            {
                direction = walkableDirections[Random.Range(0, walkableDirections.Count)];
            }
            else
            {
                //Debug.Log("no walkable directions D:");
                // this shouldn't ever happen
            }
        }

        lastInput = direction;
    }


    private void leaveBoxPath()
    {
        float distanceFromWayPoint = Vector3.Distance(transform.position, wayPoint.transform.position);

        Vector2[] possibleDirections = new Vector2[] {
            Vector2.up,
            Vector2.down,
            Vector2.left,
            Vector2.right
        };

        List<Vector2> validDirections = new List<Vector2>(possibleDirections);

        Vector2 oppositeLastInput = -lastInput;
        validDirections.Remove(oppositeLastInput);

        // sort directions by distance to the waypoint
        List<Vector2> sortedDirections = validDirections.OrderBy(dir => Vector3.Distance(transform.position + new Vector3(dir.x, dir.y, 0), wayPoint.transform.position)).ToList();

        Vector2 direction = Vector2.zero;
        bool foundPath = false;

        // find the first walkable one
        foreach (var dir in sortedDirections)
        {
            if (IsWalkable(groundTilemap.WorldToCell(transform.position + new Vector3(dir.x, dir.y, 0))))
            {
                direction = dir;
                foundPath = true;
                break;
            }
        }

        // if there isnt a shortest one just pick a random one
        if (!foundPath)
        {
            var walkableDirections = validDirections.Where(dir => IsWalkable(groundTilemap.WorldToCell(transform.position + new Vector3(dir.x, dir.y, 0)))).ToList();

            if (walkableDirections.Count > 0)
            {
                direction = walkableDirections[Random.Range(0, walkableDirections.Count)];
            }
            else
            {
                Debug.Log("no walkable directions D:");
                // this shouldn't ever happen
            }
        }

        lastInput = direction;
    }

    private void HuntingPath()
    {
        //Debug.Log(this.gameObject.name + " is hunting");`
        float distanceFromPlayer = Vector3.Distance(transform.position, pacStudent.transform.position);

        Vector2[] possibleDirections = new Vector2[] {
            Vector2.up,
            Vector2.down,
            Vector2.left,
            Vector2.right
        };

        List<Vector2> validDirections = new List<Vector2>(possibleDirections);

        Vector2 oppositeLastInput = -lastInput;
        validDirections.Remove(oppositeLastInput);

        List<Vector2> sortedDirections = validDirections.OrderBy(dir => Vector3.Distance(transform.position + new Vector3(dir.x, dir.y, 0), pacStudent.transform.position)).ToList();

        Vector2 direction = Vector2.zero;
        bool foundPath = false;

        // find the first walkable one
        foreach (var dir in sortedDirections)
        {
            if (IsWalkable(groundTilemap.WorldToCell(transform.position + new Vector3(dir.x, dir.y, 0))))
            {
                direction = dir;
                foundPath = true;
                break;
            }
        }

        // if there isnt a shortest one just pick a random one
        if (!foundPath)
        {
            var walkableDirections = validDirections.Where(dir => IsWalkable(groundTilemap.WorldToCell(transform.position + new Vector3(dir.x, dir.y, 0)))).ToList();

            if (walkableDirections.Count > 0)
            {
                direction = walkableDirections[Random.Range(0, walkableDirections.Count)];
            }
            else
            {
                Debug.Log("no walkable directions D:");
                // this shouldn't ever happen
            }
        }

        lastInput = direction;
    }


    private IEnumerator DeadGhostPath()
    {
        startPosition = transform.position;
        endPosition = spawnPosition; // Ensure spawnPosition is defined and set to where you want the ghost to go
        moveTime = 0f;
        isLerping = true;
        isRespawning = true;
        float lerpDuration = 2.0f; // Duration in seconds for the lerp to complete

        while (moveTime < lerpDuration)
        {
            moveTime += Time.deltaTime;
            float percentComplete = moveTime / lerpDuration;
            transform.position = Vector3.Lerp(startPosition, endPosition, percentComplete);
            yield return null;
        }
        transform.position = endPosition;
        yield return new WaitForSeconds(3.0f);
        isDead = false;
        isLerping = false;
        isRespawning = false;
    }

    private GameObject GetNextWayPoint() // I'm aware this is a trash way of doing this but making an array of the objects would require me to start this bit over and it works as is sooooo :p
    {
        if (mostRecentWayPoint == topLeft)
        {
            return middleTopRight;
        }
        else if (mostRecentWayPoint == topRight)
        {
            return middleRight;
        }
        else if (mostRecentWayPoint == bottomLeft)
        {
            return middleTopLeft;
        }
        else if (mostRecentWayPoint == bottomRight)
        {
            return middleBottomRight;
        }
        else if (mostRecentWayPoint == middleBottomRight)
        {
            return middleBottomLeft;
        }
        else if (mostRecentWayPoint == middleBottomLeft)
        {
            return bottomLeft;
        }
        else if (mostRecentWayPoint == middleTopRight)
        {
            return topRight;
        }
        else if (mostRecentWayPoint == middleTopLeft)
        {
            return topLeft;
        }
        else if (mostRecentWayPoint == middleRight)
        {
            return bottomRight;
        }
        else
        {
            return middleRight;
        }
    }
    private IEnumerator HuntTimer(){
        while(true){
            yield return new WaitForSeconds(difficultyManager.HuntingFrequency());
            isHunting = true;
            hudManager.huntingActive = true;
            yield return new WaitForSeconds(10f);
            isHunting = false;
            hudManager.huntingActive = false;
        }
    }

}
