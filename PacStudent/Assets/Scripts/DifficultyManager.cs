using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DifficultyManager : MonoBehaviour
{
    private int _difficulty;
    public int difficulty
    {
        get
        {
            return _difficulty;
        }
        set
        {
            _difficulty = value;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        _difficulty = PlayerPrefs.GetInt("Difficulty", 0);
    }

    public float LaserFrequency(){
        if (_difficulty == 0)
        {
            return Random.Range(30f, 60f);
        }
        else if (_difficulty == 1)
        {
            return Random.Range(20f, 40f);
        }
        else
        {
            return Random.Range(10f, 20f);
        }
    }

    public float HuntingFrequency(){
        if (_difficulty == 0)
        {
            return Random.Range(30f, 60f);
        }
        else if (_difficulty == 1)
        {
            return Random.Range(20f, 40f);
        }
        else
        {
            return Random.Range(15f, 30f);
        }
    }

    public float HerbicideFrequency(){
        if (_difficulty == 0)
        {
            return Random.Range(30f, 60f);
        }
        else if (_difficulty == 1)
        {
            return Random.Range(20f, 40f);
        }
        else
        {
            return Random.Range(15f, 30f);
        }
    }

    public int ScoreMultiplier(){
        if (_difficulty == 0)
        {
            return 1;
        }
        else if (_difficulty == 1)
        {
            return 2;
        }
        else
        {
            return 3;
        }
    }
}
