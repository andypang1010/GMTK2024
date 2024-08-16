using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    private GameState currentState;

    private void Awake() 
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

    public GameState GetState() {
        return currentState;
    }

    public void GoMenu() {
        SceneManager.LoadScene("MENU");
        currentState = GameState.MENU;
    }

    public void GoGame() {
        SceneManager.LoadScene("GAME");
        currentState = GameState.GAME;
    }

    public void GoSetting() {
        SceneManager.LoadScene("SETTINGS");
        currentState = GameState.SETTINGS;
    }

    public void GoExit() {
        Application.Quit();
    }
}

public enum GameState {
    MENU,
    GAME,
    SETTINGS,
    PAUSE
}
