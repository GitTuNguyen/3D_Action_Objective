using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUIManager : MonoBehaviour
{
    //UI state
    public enum GameUIState
    {
        GamePlay,
        Pause,
        GameOver,
        GameFinished
    }
    public GameUIState currentUIState;
    public GameObject GamePlayUI;
    public GameObject PauseUI;
    public GameObject GameOverUI;
    public GameObject GameFinishedUI;

    public TMPro.TextMeshProUGUI cointText;
    public Slider healthBarSlider;
    private void Start() 
    {        
        SwitchUIStateTo(GameUIState.GamePlay);
    }
    public void SwitchUIStateTo(GameUIState newState)
    {
        GamePlayUI.SetActive(false);
        PauseUI.SetActive(false);
        GameOverUI.SetActive(false);
        GameFinishedUI.SetActive(false);

        Time.timeScale = 1;

        switch(newState)
        {
            case GameUIState.GamePlay:
                GamePlayUI.SetActive(true);
                break;
            case GameUIState.Pause:
                Time.timeScale = 0;
                PauseUI.SetActive(true);
                break;
            case GameUIState.GameOver:
                GameOverUI.SetActive(true);
                break;
            case GameUIState.GameFinished:
                GameFinishedUI.SetActive(true);
                break;
        }
        currentUIState = newState;
    }

    public void ToggerPauseScreen()
    {
        if (currentUIState == GameUIState.GamePlay)
        {
            SwitchUIStateTo(GameUIState.Pause);
        } else if (currentUIState == GameUIState.Pause)
        {
            SwitchUIStateTo(GameUIState.GamePlay);
        }
    }

    public void Button_MainMenu()
    {
        GameManager.Instance.ReturnToMainMenu();
    }

    public void Button_Restart()
    {
        GameManager.Instance.RestartGame();
    }

    public void ShowGameOverUI()
    {
        SwitchUIStateTo(GameUIState.GameOver);
    }
    public void ShowGameFinishedUI()
    {
        SwitchUIStateTo(GameUIState.GameFinished);
    }

    public void SetHealthBarUI(float currentHeathPercent)
    {
        healthBarSlider.value = currentHeathPercent;
    }
    public void SetCoinText(int coinValue)
    {
        cointText.SetText(coinValue.ToString());
    }
}
