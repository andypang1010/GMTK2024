using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
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
    }
}
