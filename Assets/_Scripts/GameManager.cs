using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            Time.timeScale = 1f;
            pausePanel.SetActive(false);
        }

        else if (currentGameState == GameState.PAUSE) {
            Time.timeScale = 0f;
            pausePanel.SetActive(true);
        }
    }

    public void ContinueGame() {
        currentGameState = GameState.GAME;
    }

    public void PauseGame() {
        currentGameState = GameState.PAUSE;
    }
}

public enum GameState {
    GAME,
    PAUSE
}
