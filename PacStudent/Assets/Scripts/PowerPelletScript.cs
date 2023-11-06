using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerPelletScript : MonoBehaviour
{
    public AudioSource audioSource;
    private HUDManager hudManager;
    private MusicController musicController;
    // Start is called before the first frame update
    void Start()
    {
        musicController = audioSource.GetComponent<MusicController>();
        hudManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<HUDManager>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            GameObject[] ghosts = GameObject.FindGameObjectsWithTag("Enemy");
            foreach (GameObject ghost in ghosts)
            {
                GhostController controller = ghost.GetComponent<GhostController>();
                if (controller != null)
                {
                    controller.ScaredState();
                }
            }

            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                spriteRenderer.enabled = false;
            }

            BoxCollider2D boxCollider2D = GetComponent<BoxCollider2D>();
            if (boxCollider2D != null)
            {
                boxCollider2D.enabled = false;
            }

            if (musicController != null)
            {
                musicController.PlayHyperMusic();
            }

            Debug.Log("Trigger activated..");
            StartCoroutine(ScaredTimer());
        }
    }

    private IEnumerator ScaredTimer()
    {
        hudManager.StartCoroutine("HungryTimer");
        GameObject[] ghosts = GameObject.FindGameObjectsWithTag("Enemy");
        yield return new WaitForSeconds(7.0f);
        foreach (GameObject ghost in ghosts)
        {
            GhostController controller = ghost.GetComponent<GhostController>();
            if (controller != null)
            {
                controller.RecoveryState();
            }
        }
        yield return new WaitForSeconds(3.0f);
        foreach (GameObject ghost in ghosts)
        {
            GhostController controller = ghost.GetComponent<GhostController>();
            if (controller != null)
            {
                controller.NormalState();
            }
        }
        Debug.Log("Scared timer ended..");
        musicController.PlayNormalMusic();
    }

}
