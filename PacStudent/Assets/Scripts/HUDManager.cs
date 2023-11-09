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
    [Header("Warnings")]
    public bool Level2;
    public bool hungryActive;
    public bool herbActive;
    public bool laserActive;
    public bool huntingActive;
    public GameObject huntingWarning;
    public GameObject herbWarning;
    public GameObject laserWarning;
    public GameObject hungryWarning;
    private List<GameObject> activeWarnings;

    // Start is called before the first frame update
    void Start()
    {
        hungryUI = hud.transform.Find("GhostTimer").gameObject;
        hungryTimer = hungryUI.transform.Find("Time").GetComponent<TextMeshProUGUI>();
        lives = GameObject.FindGameObjectsWithTag("Lives");
        activeWarnings = new List<GameObject>();
        StartCoroutine(startCountdown());
        StartCoroutine(WarningManager());
    }

    // Update is called once per frame
    void Update()
    {
        if (Level2)
        {
            UpdateWarningList(hungryWarning, hungryActive);
            UpdateWarningList(herbWarning, herbActive);
            UpdateWarningList(laserWarning, laserActive);
            UpdateWarningList(huntingWarning, huntingActive);
        }
    }

    void UpdateWarningList(GameObject warning, bool isActive)
    {
        if (isActive && !activeWarnings.Contains(warning))
        {
            activeWarnings.Add(warning);
        }
        else if (!isActive && activeWarnings.Contains(warning))
        {
            activeWarnings.Remove(warning);
        }
    }


    public void exitButton()
    {
        SceneManager.LoadScene("StartScene");
    }

    private void ToggleHungryUI()
    {
        hungryUI.SetActive(!hungryUI.activeSelf);
        hungryActive = !hungryActive;
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
            //Debug.Log("Player is out of lives");
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

    private IEnumerator WarningManager()
    {
        while (true)
        {
            List<GameObject> cycleWarnings = new List<GameObject>(activeWarnings);
            //Debug.Log("Active Warnings: " + activeWarnings.Count + " Cycle Warnings: " + cycleWarnings.Count);
            if (cycleWarnings.Count > 1)
            {
                foreach (GameObject warning in cycleWarnings)
                {
                    warning.SetActive(true);
                    yield return new WaitForSeconds(0.5f);
                    warning.SetActive(false);
                }
            }
            else if (cycleWarnings.Count == 1)
            {
                cycleWarnings[0].SetActive(true);
                yield return new WaitForSeconds(0.5f);
                cycleWarnings[0].SetActive(false);
            }
            yield return null;
        }
    }
}
