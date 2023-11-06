using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CherryCollison : MonoBehaviour
{
    private ScoreKeeper scoreKeeper;
    private HUDManager hudManager;
    // Start is called before the first frame update
    void Start()
    {
        scoreKeeper = GameObject.FindGameObjectWithTag("GameController").GetComponent<ScoreKeeper>();
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
            scoreKeeper.AddScore(100);
            Destroy(this.gameObject);
        }
    }
}
