using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CherryCollison : MonoBehaviour
{
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
        if (other.gameObject.CompareTag("Player"))
        {
            // Add 100 points to the score
            // Destroy the cherry
            Destroy(this.gameObject);
        }
    }
}
