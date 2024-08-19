using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameState currentGameState { get; private set; }
    public GameObject currentLevel;
    public GameObject[] allLevels;
    public GameObject teaLake;
    public int[] defaultLevelScales = { 1, 1 };
    private GameObject player;


    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;

            // DontDestroyOnLoad(gameObject);
        }
    }

    void Start()
    {
        Instance.currentGameState = GameState.GAME;
        player = GameObject.Find("PLAYER");
    }

    void Update()
    {
        foreach (GameObject level in allLevels) {
            if (level == currentLevel) {
                foreach (Transform element in level.transform) {
                    element.gameObject.SetActive(true);
                }
            }
            else {
                foreach (Transform element in level.transform) {
                    element.gameObject.SetActive(false);
                }
            }
        }
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

    public void ResetLevel() {
        Time.timeScale = 1f;

        print("Level reset");
        if (currentLevel == null) {
            print("Currently at lobby");
            return;
        }

        foreach (Transform element in currentLevel.transform) {
            if (element.TryGetComponent(out Scalable scalable)) {
                scalable.ResetScale();
            }

            if (element.TryGetComponent(out NPCMovement npcMovement)) {
                npcMovement.ResetPosition();
            }
            
            if (element.TryGetComponent(out Elevator elevator)) {
                elevator.ResetElevator();
            }
        }

        player.GetComponent<PlayerMovement>().ResetLevelPosition(
            currentLevel.transform.Find("Player Reset Spawn").transform.position);

        player.GetComponent<PlayerScale>().ResetPlayerScale();

        currentGameState = GameState.GAME;

        GameObject.Find("AUDIO MANAGER").GetComponent<BGMManager>().ResetAudio();
    }

    public void DrainTea() {
        teaLake.SetActive(false);
    }
}

public enum GameState
{
    GAME,
    PAUSE
}
