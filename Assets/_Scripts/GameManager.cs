using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameState currentGameState { get; private set; }
    public int[] defaultLevelScales = { 1, 1 };


    void Awake()
    {
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

    public void PlacePlayerInLevel(int levelIndex, int fromLevelIndex, Transform toPosition)
    {
        GameObject player = GameObject.Find("PLAYER");
        Debug.Log("Placing player in level " + levelIndex + " from level " + fromLevelIndex);
        player.transform.position = toPosition.position;
        player.transform.localScale = Vector3.one * defaultLevelScales[levelIndex];
    }
}

public enum GameState
{
    GAME,
    PAUSE
}
