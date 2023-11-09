using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScissorController : MonoBehaviour
{
    private Camera sceneCamera;
    public GameObject scissorPrefab;
    public float moveSpeed = 2f;
    private GameObject currentScissor = null;

    // Start is called before the first frame update
    void Start()
    {
        sceneCamera = Camera.main;
        StartCoroutine(IntitalScissorCall());
    }

    public Vector2 GetRandomPosition()
    {
        Vector3[] positions = new Vector3[]
        {
            // All positions that will cross the middle of the screen
            new Vector3(0, 0, sceneCamera.nearClipPlane), // bottom left
            new Vector3(1, 0, sceneCamera.nearClipPlane), // bottom right
            new Vector3(0, 1, sceneCamera.nearClipPlane), // top left
            new Vector3(1, 1, sceneCamera.nearClipPlane), // top right
            new Vector3(0, 0.5f, sceneCamera.nearClipPlane), // left
            new Vector3(0.5f, 0, sceneCamera.nearClipPlane), // bottom
            new Vector3(1, 0.5f, sceneCamera.nearClipPlane), // right
            new Vector3(0.5f, 1, sceneCamera.nearClipPlane), // top
        };

        Vector3 randomPosition = positions[Random.Range(0, positions.Length)];
        Vector3 worldPosition = sceneCamera.ViewportToWorldPoint(randomPosition);
        worldPosition.z = 0;

        return worldPosition;
    }

    // Spawn a new scissor every 10 seconds after the last one has been destrtoyed
    IEnumerator SpawnScissor()
    {
        yield return new WaitForSeconds(5.0f);
        while (true)
        {
            Vector2 spawnPosition = GetRandomPosition();
            Vector2 targetPosition = GetOppositePosition(spawnPosition);

            Vector2 direction = (targetPosition - spawnPosition).normalized;

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            Quaternion rotation = Quaternion.Euler(0, 0, angle);

            currentScissor = Instantiate(scissorPrefab, spawnPosition, rotation);

            yield return StartCoroutine(MoveScissor(currentScissor, targetPosition));

            //yield return new WaitForSeconds(10f);
        }
    }


    // Move scissor to oposite side of screen
    private IEnumerator MoveScissor(GameObject scissor, Vector2 targetPosition)
    {
        while (scissor != null && (Vector2)scissor.transform.position != targetPosition)
        {
            if (scissor == null) yield break;
            scissor.transform.position = Vector2.MoveTowards(scissor.transform.position, targetPosition, moveSpeed * Time.deltaTime);
            yield return null;
        }
        if (scissor != null) Destroy(scissor);
    }

    private Vector2 GetOppositePosition(Vector2 originalPosition)
    {
        Vector2 viewportPosition = sceneCamera.WorldToViewportPoint(originalPosition);
        Vector2 oppositeViewportPosition = new Vector2(1 - viewportPosition.x, 1 - viewportPosition.y);
        Vector3 worldPosition = sceneCamera.ViewportToWorldPoint(new Vector3(oppositeViewportPosition.x, oppositeViewportPosition.y, 0));
        worldPosition.z = 0;
        return worldPosition;
    }

    private IEnumerator IntitalScissorCall(){
        StartCoroutine(SpawnScissor());
        yield return new WaitForSeconds(5.0f);
        StartCoroutine(SpawnScissor());
        yield return new WaitForSeconds(5.0f);
        StartCoroutine(SpawnScissor());
        yield return new WaitForSeconds(5.0f);
        StartCoroutine(SpawnScissor());
    }
}
