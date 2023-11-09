using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScissorCollider : MonoBehaviour
{
    private HUDManager hudManager;
    // Start is called before the first frame update
    void Start()
    {
        hudManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<HUDManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (hudManager.gameStarted == false)
        {
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.GetComponent<PacStudentController>().KillPlayer();
            Destroy(this.gameObject);
        }
    }
}
