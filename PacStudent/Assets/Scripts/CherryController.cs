using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;

public class CherryController : MonoBehaviour
{
    private Camera sceneCamera;
    public GameObject cherryPrefab;
    public float moveSpeed = 2f;
    private GameObject currentCherry = null;

    // Start is called before the first frame update
    void Start()
    {
        sceneCamera = Camera.main;
        StartCoroutine(SpawnCherry());
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

    // Spawn a new cherry every 10 seconds after the last one has been destrtoyed
    IEnumerator SpawnCherry()
    {
        while (true)
        {
            Vector2 spawnPosition = GetRandomPosition();
            currentCherry = Instantiate(cherryPrefab, spawnPosition, Quaternion.identity);

            Vector2 targetPosition = GetOppositePosition(spawnPosition);

            yield return StartCoroutine(MoveCherry(currentCherry, targetPosition));

            yield return new WaitForSeconds(10f);
        }
    }

    // Move cherry to oposite side of screen
    private IEnumerator MoveCherry(GameObject cherry, Vector2 targetPosition)
    {
        while ( cherry != null && (Vector2)cherry.transform.position != targetPosition)
        {
            if (cherry == null) yield break;
            cherry.transform.position = Vector2.MoveTowards(cherry.transform.position, targetPosition, moveSpeed * Time.deltaTime);
            yield return null;
        }
        if (cherry != null) Destroy(cherry);
    }

    private Vector2 GetOppositePosition(Vector2 originalPosition)
    {
        Vector2 viewportPosition = sceneCamera.WorldToViewportPoint(originalPosition);
        Vector2 oppositeViewportPosition = new Vector2(1 - viewportPosition.x, 1 - viewportPosition.y);
        Vector3 worldPosition = sceneCamera.ViewportToWorldPoint(new Vector3(oppositeViewportPosition.x, oppositeViewportPosition.y, 0));
        worldPosition.z = 0;
        return worldPosition;
    }
}
