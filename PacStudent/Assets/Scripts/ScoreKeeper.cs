using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreKeeper : MonoBehaviour
{
    private int _score;
    public int score
    {
        get
        {
            return _score;
        }
        set
        {
            _score = value;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        _score = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddScore(int points)
    {
        HUDManager hudManager = GetComponent<HUDManager>();
        _score += points;
        hudManager.UpdateScore(score);
    }
}
