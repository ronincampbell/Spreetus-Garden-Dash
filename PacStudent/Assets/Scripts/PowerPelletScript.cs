using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerPelletScript : MonoBehaviour
{
    public AudioSource audioSource;
    private MusicController musicController;
    // Start is called before the first frame update
    void Start()
    {
        musicController = audioSource.GetComponent<MusicController>();
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

            // Disable the sprite renderer only once, after all ghosts have been set to scared state.
            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                spriteRenderer.enabled = false;
            }

            // Assuming you want to play the hyperMusic when the power pellet is consumed.
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
