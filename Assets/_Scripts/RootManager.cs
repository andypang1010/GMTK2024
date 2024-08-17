using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RootManager : MonoBehaviour
{
    public static RootManager Instance;
    public int frameRate = 60;
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

    private void Update()
    {
        Application.targetFrameRate = frameRate;

        if (SceneManager.GetActiveScene().name == "MENU"
        || SceneManager.GetActiveScene().name == "SETTINGS") {
            Destroy(GameObject.Find("GAME MANAGER"));
        }
    }

    public void GoMenu()
    {
        SceneManager.LoadScene("MENU");
    }

    public void GoGame()
    {
        SceneManager.LoadScene("LEVEL 1");
    }

    public void GoSetting()
    {
        SceneManager.LoadScene("SETTINGS");
    }

    public void GoExit()
    {
        Application.Quit();
    }

    public void GoToScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
