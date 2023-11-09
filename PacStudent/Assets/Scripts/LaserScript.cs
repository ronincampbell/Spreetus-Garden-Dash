using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;

//[RequireComponent(typeof(LineRenderer))]
public class LaserScript : MonoBehaviour
{
    public Material laserMaterial;
    public GameObject laserParticleSystem;
    private LineRenderer lineRenderer;
    public Camera sceneCamera;
    public GameObject laserWarning;
    private Vector2 spawnPosition;
    private Vector2 targetPosition;
    private BoxCollider2D boxCollider;
    private GameObject startParticle = null;
    private GameObject endParticle = null;
    private HUDManager hudManager;
    public AudioClip laserSound;
    public AudioSource audioSource;
    private DifficultyManager difficultyManager;


    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        boxCollider = gameObject.AddComponent<BoxCollider2D>();
        boxCollider.isTrigger = true;
        boxCollider.enabled = false;
        hudManager = GameObject.FindWithTag("GameController").GetComponent<HUDManager>();
        difficultyManager = GameObject.FindWithTag("GameController").GetComponent<DifficultyManager>();

        // Set the number of points (just 2 for a single segment)
        lineRenderer.positionCount = 2;

        // Set the material to the LineRenderer
        //lineRenderer.material = laserMaterial;

        // Set the width of the LineRenderer
        //lineRenderer.startWidth = 1f;
        //lineRenderer.endWidth = 1f;
        //lineRenderer.SetPosition(0, startPoint.position);
        //lineRenderer.SetPosition(1, endPoint.position);
        StartCoroutine(LaserTimer());
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

    private IEnumerator LaserTimer()
    {
        yield return new WaitForSeconds(10f);
        while (true)
        {
            yield return new WaitForSeconds(difficultyManager.LaserFrequency());
            StartCoroutine(CastLaser());
        }
    }

    private IEnumerator CastLaser()
    {
        hudManager.laserActive = true;
        audioSource.PlayOneShot(laserSound);
        yield return new WaitForSeconds(1f);
        spawnPosition = GetRandomPosition();
        targetPosition = GetOppositePosition(spawnPosition);
        startParticle = Instantiate(laserParticleSystem, spawnPosition, Quaternion.identity);
        endParticle = Instantiate(laserParticleSystem, targetPosition, Quaternion.identity);
        yield return new WaitForSeconds(2f);
        boxCollider.enabled = true;
        lineRenderer.enabled = true;
        lineRenderer.SetPosition(0, spawnPosition);
        lineRenderer.SetPosition(1, targetPosition);
        UpdateCollider();

        yield return new WaitForSeconds(3f);
        Destroy(startParticle);
        Destroy(endParticle);
        lineRenderer.enabled = false;
        boxCollider.enabled = false;
        hudManager.laserActive = false;
    }

    private Vector2 GetOppositePosition(Vector2 originalPosition)
    {
        Vector2 viewportPosition = sceneCamera.WorldToViewportPoint(originalPosition);
        Vector2 oppositeViewportPosition = new Vector2(1 - viewportPosition.x, 1 - viewportPosition.y);
        Vector3 worldPosition = sceneCamera.ViewportToWorldPoint(new Vector3(oppositeViewportPosition.x, oppositeViewportPosition.y, 0));
        worldPosition.z = 0;
        return worldPosition;
    }

    private void UpdateCollider()
    {
        Vector2 direction = (targetPosition - spawnPosition).normalized;
        Vector2 localMidpoint = transform.InverseTransformPoint((spawnPosition + targetPosition) / 2);

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        boxCollider.offset = localMidpoint;

        boxCollider.size = new Vector2(Vector2.Distance(spawnPosition, targetPosition), 1f);

        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<PacStudentController>().KillPlayer();
        }
    }
}
