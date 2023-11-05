using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleporterScript : MonoBehaviour
{
    public bool isLeft;
    public GameObject linkedTeleporter;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Detected..");
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player detected..");

            // Interrupt the lerp and teleport the player
            PacStudentController controller = other.GetComponent<PacStudentController>();
            if (controller != null)
            {
                controller.InterruptLerpAndTeleport(linkedTeleporter.transform.position, isLeft);
            }
        }
    }

}
