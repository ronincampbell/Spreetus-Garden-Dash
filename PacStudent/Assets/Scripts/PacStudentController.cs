using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;

public class PacStudentController : MonoBehaviour
{
    private bool isWaiting;
    private Animator animator;
    private AudioSource audioSource;
    public AudioClip pelletSFX;
    private Vector2 lastInput;
    private Vector2 currentInput;
    private Vector3 startPosition;
    public float moveSpeed = 5.0f;
    public Tilemap groundTilemap;
    public AudioClip walkSFX;
    private Vector3 endPosition;
    private float moveTime;
    private bool isLerping;


    /*
    ---- NOTE ----
    I used the actual tilemap to handle to logic and update positions
    instead of the LevelGenerator.cs script cause i didn't completely finish it in the first part :p
    --------------
    */


    // Start is called before the first frame update
    void Start()
    {
        currentInput = lastInput = Vector2.right;
        startPosition = endPosition = transform.position;
        isLerping = isWaiting = false;
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
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
                AudioClip clipToPlay = GetClipForNextTile(endPosition);
                StartCoroutine(PlayMovingSoundWithDelay(clipToPlay));
            }
        }
    }

    private void GatherInput() // Setting each key to an direction
    {
        if (Input.GetKeyDown(KeyCode.W)) lastInput = Vector2.up;
        if (Input.GetKeyDown(KeyCode.A)) lastInput = Vector2.left;
        if (Input.GetKeyDown(KeyCode.S)) lastInput = Vector2.down;
        if (Input.GetKeyDown(KeyCode.D)) lastInput = Vector2.right;
    }

    private void TryMove(Vector2 direction)
    {
        // get the next cell
        Vector3Int nextCell = groundTilemap.WorldToCell(transform.position + new Vector3(direction.x, direction.y, 0));

        if (IsWalkable(nextCell)) // only move if the cell is walkable
        {
            currentInput = direction;
            StartLerp(GetCellCenterWorld(nextCell));
        }
        else if (currentInput != Vector2.zero) // check if we can keep going
        {
            nextCell = groundTilemap.WorldToCell(transform.position + new Vector3(currentInput.x, currentInput.y, 0));
            if (IsWalkable(nextCell))
            {
                StartLerp(GetCellCenterWorld(nextCell));
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

    private AudioClip GetClipForNextTile(Vector3 position) // check if theres a pellet or not
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
}
