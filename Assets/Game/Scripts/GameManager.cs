using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public Character playerCharacter;
    public HealthManager healthManager;
    public GameUIManager gameUIManager;
    bool isGameOver;
    bool isGameFinished;
    private void Awake() {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    void Update()
    {
        if (isGameOver)
        {
            return;
        }
        if (playerCharacter.CurrentState == Character.CharacterState.Dead)
        {
            GameOver();
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            gameUIManager.ToggerPauseScreen();
        }
        gameUIManager.SetCoinText(playerCharacter.coin);
        gameUIManager.SetHealthBarUI(healthManager.CurrentHealthPercent());
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("MainMenu");
    }
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void GameOver()
    {
        isGameOver = true;
        gameUIManager.ShowGameOverUI();
        print("GAME OVER");
    }
    public void GameFinished()
    {
        gameUIManager.ShowGameFinishedUI();
        print("Finished Game");
    }
}
