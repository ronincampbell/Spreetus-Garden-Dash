using System.Collections;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using UnityEngine;

public class PacStudentController : MonoBehaviour
{
    [Header("Audio")]
    private AudioSource audioSource;
    public AudioClip pelletSFX;
    public AudioClip wallSFX;
    public AudioClip walkSFX;
    public AudioClip deathSFX;
    [Header("Graphics")]
    public Tilemap groundTilemap;
    public Tile emptyGroundTile;
    public ParticleSystem wallBump;
    public GameObject movementParticles;
    [Header("Game Values")]
    public GameObject gameManager;
    private HUDManager hudManager;
    private ScoreKeeper scoreKeeper;
    public float moveSpeed = 5.0f;
    private float moveTime;
    private int eatenPellets = 0;
    private bool isWaiting;
    private bool isLerping;
    public bool isDead;
    private Vector2 lastInput;
    private Vector2 currentInput;
    private Vector3 SpawnPosition;
    private Vector3 startPosition;
    private Vector3 endPosition;
    private Animator animator;

    /*
    ---- NOTE ----
    I used the actual tilemap to handle to logic and update positions
    instead of the LevelGenerator.cs script cause i didn't completely finish it in the first part :p
    --------------
    */

    // His name is spritus

    // Start is called before the first frame update
    void Start()
    {
        currentInput = lastInput = Vector2.zero;
        startPosition = endPosition = transform.position;
        isLerping = isWaiting = false;
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        SpawnPosition = transform.position;
        hudManager = gameManager.GetComponent<HUDManager>();
        scoreKeeper = gameManager.GetComponent<ScoreKeeper>();
    }

    // Update is called once per frame
    void Update()
    {
        if (hudManager.gameStarted)
        {
            //Debug.Log(GetCellCenterWorld(groundTilemap.WorldToCell(transform.position)));
            GatherInput();

            if (!isLerping)
            {
                TryMove(lastInput);
            }

            if (isLerping)
            {
                ContinueLerp();
                PlayerDirectionAnimation(currentInput);
                if (!audioSource.isPlaying && !isWaiting)
                {
                    AudioClip clipToPlay = GetClipToPlay(endPosition);
                    StartCoroutine(PlayMovingSoundWithDelay(clipToPlay));
                }
            }
            CheckForPellet(transform.position);
        }

    }

    private void GatherInput() // Setting each key to an direction
    {
        if (!isDead)
        {
            if (Input.GetKeyDown(KeyCode.W)) lastInput = Vector2.up;
            if (Input.GetKeyDown(KeyCode.A)) lastInput = Vector2.left;
            if (Input.GetKeyDown(KeyCode.S)) lastInput = Vector2.down;
            if (Input.GetKeyDown(KeyCode.D)) lastInput = Vector2.right;
        }
    }

    private void TryMove(Vector2 direction)
    {
        // get the next cell
        Vector3Int nextCell = groundTilemap.WorldToCell(transform.position + new Vector3(direction.x, direction.y, 0));

        if (IsWalkable(nextCell)) // only move if the cell is walkable
        {
            currentInput = direction;
            StartLerp(GetCellCenterWorld(nextCell));
            if (!movementParticles.activeSelf)
            {
                movementParticles.SetActive(true);
            }
        }
        else if (currentInput != Vector2.zero) // check if we can keep going
        {
            nextCell = groundTilemap.WorldToCell(transform.position + new Vector3(currentInput.x, currentInput.y, 0));
            if (IsWalkable(nextCell))
            {
                StartLerp(GetCellCenterWorld(nextCell));
            }
            else // This condition will only be called when hitting a wall
            {
                wallBump.transform.position = (GetCellCenterWorld(nextCell) + transform.position) / 2;
                //Debug.Log("Called");
                currentInput = Vector2.zero;
                movementParticles.SetActive(false);
                audioSource.PlayOneShot(wallSFX, 1.0f);
                wallBump.Play();

            }
        }
    }

    private Vector3 GetCellCenterWorld(Vector3Int cellPosition) // i had to add this cause other wise he goes to the 0,0 of the cell which is a corner
    {
        return groundTilemap.GetCellCenterWorld(cellPosition);
    }

    private bool IsWalkable(Vector3Int cellPosition) // return true if the cell is walkable
    {
        return groundTilemap.HasTile(cellPosition);
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

    private void PlayerDirectionAnimation(Vector2 direction)
    {
        // Player direction identification and animation
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            if (direction.x > 0)
            {
                animator.Play("WalkRightAnim");
            }
            else
            {
                animator.Play("WalkLeftAnim");
            }
        }
        else if (Mathf.Abs(direction.y) > 0)
        {
            if (direction.y > 0)
            {
                animator.Play("WalkUpAnim");
            }
            else
            {
                animator.Play("WalkDownAnim");
            }
        }
    }

    // play the required audioCilp
    private IEnumerator PlayMovingSoundWithDelay(AudioClip clip)
    {
        if (!isLerping) yield break;
        isWaiting = true;
        yield return new WaitForSeconds(0.1f);
        audioSource.clip = clip;
        audioSource.Play();
        isWaiting = false;
    }

    private void CheckForPellet(Vector3 position) // check if theres a pellet or not
    {
        Vector3Int cellPosition = groundTilemap.WorldToCell(position);
        Tile tile = groundTilemap.GetTile<Tile>(cellPosition);
        if (tile != null && tile.name == "TileSheetFinal_7")
        {
            // This removes the pellet from the tile - I know that it says to use colliders but that would need me to recreate the level and make a new prefab, this is just a more elegant solution. Plus, i used them for the other stuff :p
            groundTilemap.SetTile(cellPosition, emptyGroundTile);

            scoreKeeper.AddScore(10);
            eatenPellets++;

        }
        if (eatenPellets == 220){
            hudManager.GameOver();
        }
    }
    private AudioClip GetClipToPlay(Vector3 position) // get the right clip to play for the tile
    {
        Vector3Int cellPosition = groundTilemap.WorldToCell(position);
        Tile tile = groundTilemap.GetTile<Tile>(cellPosition);
        if (tile != null && tile.name == "TileSheetFinal_7")
        {
            return pelletSFX;
        }
        else
        {
            return walkSFX;
        }
    }


    // logic to allow teleporting
    public void InterruptLerpAndTeleport(Vector3 newPosition, bool isLeft)
    {
        isLerping = false;

        if (movementParticles.activeSelf)
        {
            movementParticles.SetActive(false);
        }

        if (isLeft)
        {
            newPosition.x -= 1.5f;
        }
        else
        {
            newPosition.x += 1.5f;
        }
        newPosition.y = transform.position.y;
        transform.position = newPosition;
    }

    public void KillPlayer()
    {

        isLerping = false;
        if (movementParticles.activeSelf)
        {
            movementParticles.SetActive(false);
        }
        isDead = true;
        animator.Play("DeathAnim");
        audioSource.PlayOneShot(deathSFX, 1.0f);
        lastInput = currentInput = Vector2.zero;
        hudManager.LoseLife();
        GetComponent<Collider2D>().enabled = false;
    }

    public void Respawn()
    {
        if (!hudManager.gameStarted) return;
        isDead = false;
        currentInput = lastInput = Vector2.zero;
        startPosition = endPosition = transform.position;
        isLerping = isWaiting = false;
        transform.position = SpawnPosition;
        animator.Play("WalkLeftAnim");
        GetComponent<Collider2D>().enabled = true;
    }

}
