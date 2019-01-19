using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class PongManager : MonoBehaviour
{
    public static PongManager Instance;
    public TextMeshProUGUI[] scoreTexts = new TextMeshProUGUI[2];
    public GameObject pauseMenu;

    private int[] scores = new int[2];
    

    private void Awake()
    {
        if (Instance != null) Destroy(this);
        else Instance = this;
    }

    public void PlayerScored(int player)
    {
        scores[player]++;
        scoreTexts[player].text = "Scoreth: " + scores[player];
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PauseGame();
        }
    }

    public void PauseGame()
    {
        Time.timeScale = 0;
        pauseMenu.SetActive(true);
    }

    public void ResumeGame()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1;
    }

    public void QuitToMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("Title");
    }
    
}
