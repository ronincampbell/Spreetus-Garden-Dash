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
    private bool isLevel2;
    private DifficultyManager difficultyManager;
    // Start is called before the first frame update
    void Start()
    {
        _score = 0;
        isLevel2 = GetComponent<HUDManager>().Level2;
        if (isLevel2)
        {
            difficultyManager = GetComponent<DifficultyManager>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddScore(int points)
    {
        HUDManager hudManager = GetComponent<HUDManager>();
        if (isLevel2)
        {
            points *= difficultyManager.ScoreMultiplier();
        }
        _score += points;
        hudManager.UpdateScore(score);
    }
}
