using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MenuManager : MonoBehaviour
{
    public TextMeshProUGUI highScore;

    // Start is called before the first frame update
    private void Start() {
        //For Debugging
        //PlayerPrefs.DeleteAll();
        //PlayerPrefs.Save();
        float highScoreTime = PlayerPrefs.GetFloat("HighScoreTime", 0.0f);
        if (highScoreTime == 0.0f)
        {
            highScore.text = "PLAY TO SET A HIGH SCORE!";
            return;
        }
        int minutes = Mathf.FloorToInt(highScoreTime / 60F);
        int seconds = Mathf.FloorToInt(highScoreTime % 60F);
        int milliseconds = Mathf.FloorToInt((highScoreTime * 100F) % 100F);
        string formatedTime = string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, milliseconds);
        highScore.text = "HIGH SCORE: " + PlayerPrefs.GetInt("HighScore").ToString() + " IN " + formatedTime;
    }

    public void LoadLevel1()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Level1");
    }
}
