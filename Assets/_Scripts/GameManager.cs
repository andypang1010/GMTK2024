using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public GameObject pausePanel;
    private GameState currentGameState;

    void Start() {
        currentGameState = GameState.GAME;
    }

    void Update()
    {
        if (currentGameState == GameState.GAME) {
            pausePanel.SetActive(false);
        }

        else if (currentGameState == GameState.PAUSE) {
            pausePanel.SetActive(true);
        }
    }

    public void ContinueGame() {
        currentGameState = GameState.GAME;
        Time.timeScale = 1f;
    }

    public void PauseGame() {
        currentGameState = GameState.PAUSE;
        Time.timeScale = 0f;
    }
}

public enum GameState {
    GAME,
    PAUSE
}
