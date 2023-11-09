using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HerbicideController : MonoBehaviour
{
    private GameObject player;
    private float playerDefaultMoveSpeed;
    private ParticleSystem herbGas;
    private HUDManager hudManager;
    public AudioClip herbicideSound;
    public AudioSource audioSource;
    private DifficultyManager difficultyManager;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player");
        hudManager = GameObject.FindWithTag("GameController").GetComponent<HUDManager>();
        playerDefaultMoveSpeed = player.GetComponent<PacStudentController>().moveSpeed;
        difficultyManager = GameObject.FindWithTag("GameController").GetComponent<DifficultyManager>();
        herbGas = GetComponent<ParticleSystem>();
        StartCoroutine(herbicideTimer());
    }

    private IEnumerator herbicideTimer()
    {
        yield return new WaitForSeconds(30.0f);
        while (true)
        {
            // herbicide warning
            hudManager.herbActive = true;
            audioSource.PlayOneShot(herbicideSound);
            yield return new WaitForSeconds(1.0f);
            herbGas.Play();
            yield return new WaitForSeconds(3.0f);
            player.GetComponent<PacStudentController>().moveSpeed = 1.5f;
            player.GetComponent<PacStudentController>().invertControls = true;
            yield return new WaitForSeconds(7.0f);
            player.GetComponent<PacStudentController>().moveSpeed = playerDefaultMoveSpeed;
            player.GetComponent<PacStudentController>().invertControls = false;
            herbGas.Stop();
            hudManager.herbActive = false;

            yield return new WaitForSeconds(difficultyManager.HerbicideFrequency());
        }
    }


}
