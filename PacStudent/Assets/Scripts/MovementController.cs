using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController : MonoBehaviour
{
    public Vector3[] targetPositions;
    public float speed = 5.0f;
    public AudioClip walkSFX;

    private int currentTargetIndex = 0;
    private Animator animator;
    private AudioSource audioSource;
    private bool isWaiting = false;

    // Start is called before the first frame update
    private void Start()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    private void Update()
    {
        Vector3 currentTarget = targetPositions[currentTargetIndex];
        
        float step = speed * Time.deltaTime;
        float distanceToTarget = Vector3.Distance(transform.position, currentTarget);

        // Direction to target
        Vector3 direction = (currentTarget - transform.position).normalized;

        PlayerDirectionAnimation(direction);

        // Play walk sound
        if (!audioSource.isPlaying && !isWaiting)
        {
            StartCoroutine(PlayMovingSoundWithDelay());
        }

        // If close to waypoint, change target waypoint
        if (step >= distanceToTarget)
        {
            transform.position = currentTarget;
            currentTargetIndex = (currentTargetIndex + 1) % targetPositions.Length;
        }
        else
        {
            // Move towards waypoint
            float moveFraction = step / distanceToTarget;
            transform.position = Vector3.Lerp(transform.position, currentTarget, moveFraction);
        }
    }

    private void PlayerDirectionAnimation(Vector3 direction)
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
        else
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

    // Delay for walk SFX
    private IEnumerator PlayMovingSoundWithDelay()
    {
        isWaiting = true;
        yield return new WaitForSeconds(0.1f);
        audioSource.clip = walkSFX;
        audioSource.Play();
        isWaiting = false;
    }
}
