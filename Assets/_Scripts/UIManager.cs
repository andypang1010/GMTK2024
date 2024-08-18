using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject resetButton;
    public GameObject pausePanel;
    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        if (GameManager.Instance.currentGameState == GameState.GAME)
        {
            pausePanel.SetActive(false);
        }

        else if (GameManager.Instance.currentGameState == GameState.PAUSE)
        {
            pausePanel.SetActive(true);
        }

        if (GameManager.Instance.currentLevel == null) {
            resetButton.SetActive(false);
        }

        else {
            resetButton.SetActive(true);
        }
    }
}
