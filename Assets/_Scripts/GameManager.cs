using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameState currentGameState { get; private set; }

    void Awake() {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    void Start()
    {
        Instance.currentGameState = GameState.GAME;
    }

    void Update()
    {

    }

    public void ContinueGame()
    {
        Instance.currentGameState = GameState.GAME;
        Time.timeScale = 1f;
    }

    public void PauseGame()
    {
        Instance.currentGameState = GameState.PAUSE;
        Time.timeScale = 0f;
    }
}

public enum GameState
{
    GAME,
    PAUSE
}
