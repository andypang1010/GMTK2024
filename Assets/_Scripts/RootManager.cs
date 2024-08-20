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
    }

    public void GoMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MENU");
    }

    public void GoOpeningCG() {
        Time.timeScale = 1f;
        SceneManager.LoadScene("OPENING CG");
    }

    public void GoTutorial() {
        Time.timeScale = 1f;
        SceneManager.LoadScene("TUTORIAL");
    }

    public void GoGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("GAME");
    }

    public void GoExit()
    {
        Application.Quit();
    }

    public void GoToScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void GoToScene(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }
}
