using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class HUDManager : MonoBehaviour
{
    public GameObject hud;
    private GameObject hungryUI;
    private GameObject[] lives;
    private TextMeshProUGUI hungryTimer;
    private int remainingLives = 3;
    private float timer = 0.0f;
    private int playerScore = 0;
    public bool gameStarted = false;
    // Start is called before the first frame update
    void Start()
    {
        hungryUI = hud.transform.Find("GhostTimer").gameObject;
        hungryTimer = hungryUI.transform.Find("Time").GetComponent<TextMeshProUGUI>();
        lives = GameObject.FindGameObjectsWithTag("Lives");
        StartCoroutine(startCountdown());
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void exitButton()
    {
        SceneManager.LoadScene("StartScene");
    }

    private void ToggleHungryUI()
    {
        hungryUI.SetActive(!hungryUI.activeSelf);
    }

    public IEnumerator HungryTimer()
    {
        ToggleHungryUI();
        float countdown = 10.0f;
        while (countdown > 0)
        {
            hungryTimer.text = countdown.ToString("0") + " SECONDS LEFT";
            yield return new WaitForSeconds(1.0f);
            countdown--;
        }
        //hungryTimer.text = "0 SECONDS LEFT";
        ToggleHungryUI();
    }

    public void LoseLife()
    {
        remainingLives--;
        if (remainingLives > 0)
        {
            lives[remainingLives].SetActive(false);
        }
        else
        {
            Debug.Log("Player is out of lives");
            lives[0].SetActive(false);
            GameOver();
        }
    }

    public void UpdateScore(int score)
    {
        TextMeshProUGUI scoreText = hud.transform.Find("Score").transform.Find("Points").GetComponent<TextMeshProUGUI>();
        playerScore += score;
        scoreText.text = score.ToString();
    }

    public IEnumerator startCountdown()
    {
        GameObject startUI = hud.transform.Find("StartUI").gameObject;
        TextMeshProUGUI timerText = startUI.transform.Find("StartCountdown").GetComponent<TextMeshProUGUI>();
        float countdown = 3.0f;
        while (countdown > 0)
        {
            timerText.text = countdown.ToString();
            yield return new WaitForSeconds(1.0f);
            countdown--;
        }
        timerText.text = "GO!";
        yield return new WaitForSeconds(1.0f);
        startUI.SetActive(false);
        gameStarted = true;
        StartCoroutine(startTimer());
    }

    private IEnumerator startTimer()
    {
        TextMeshProUGUI gameTimerText = hud.transform.Find("GameTimer").transform.Find("Time").GetComponent<TextMeshProUGUI>();
        while (gameStarted)
        {
            timer += Time.deltaTime;
            int minutes = Mathf.FloorToInt(timer / 60F);
            int seconds = Mathf.FloorToInt(timer % 60F);
            int milliseconds = Mathf.FloorToInt((timer * 100F) % 100F);

            // Update the label value
            gameTimerText.text = string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, milliseconds);

            yield return null; // Yield until the next frame
        }
        yield return null;
    }

    public void GameOver()
    {
        gameStarted = false;

        GameObject gameOverUI = hud.transform.Find("GameOverUI").gameObject;
        gameOverUI.SetActive(true);

        if (PlayerPrefs.GetFloat("HighScoreTime") < timer)
        {
            PlayerPrefs.SetFloat("HighScoreTime", timer);
        }

        if (PlayerPrefs.GetInt("HighScore") < playerScore)
        {
            PlayerPrefs.SetInt("HighScore", playerScore);
        }

        PlayerPrefs.Save();

        StartCoroutine(GameOverTimer());
    }

    private IEnumerator GameOverTimer()
    {
        yield return new WaitForSeconds(3.0f);
        SceneManager.LoadScene("StartScene");
    }
}
