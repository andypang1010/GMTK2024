using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject pausePanel;
    private GameState currentGameState;

    void Start()
    {
        currentGameState = GameState.GAME;
    }

    void Update()
    {
        if (currentGameState == GameState.GAME)
        {
            pausePanel.SetActive(false);
        }

        else if (currentGameState == GameState.PAUSE)
        {
            pausePanel.SetActive(true);
        }
    }

    public void ContinueGame()
    {
        currentGameState = GameState.GAME;
        Time.timeScale = 1f;
    }

    public void PauseGame()
    {
        currentGameState = GameState.PAUSE;
        Time.timeScale = 0f;
    }

    public void QuitToMenu()
    {
        Time.timeScale = 1f;
        // Load Menu Scene
        SceneManager.LoadScene(0);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}

public enum GameState
{
    GAME,
    PAUSE
}
