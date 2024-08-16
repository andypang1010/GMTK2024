using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PorridgeGameManager : MonoBehaviour
{
    // Singleton for game manager
    public static PorridgeGameManager instance;
    public GameObject goal;
    public TextMeshProUGUI winText;
    public GameObject player;
    public GameObject gameSpace;

    // struct for game state
    public struct GameState
    {
        public bool isGameOver;
        public bool isGameWon;
    }
    private GameState gameState;


    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        gameState.isGameOver = false;
        gameState.isGameWon = false;
        winText.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (goal.GetComponent<Goal>().isGameWon)
        {
            gameState.isGameWon = true;
            gameState.isGameOver = true;
            winText.gameObject.SetActive(true);
        }

        if (gameSpace.GetComponent<KillThingsOut>().shouldRestart)
        {
            gameState.isGameOver = true;
            winText.text = "You fell out of the game space!";
            winText.gameObject.SetActive(true);
        }

        if (player.GetComponent<PlayerController>().health <= 0)
        {
            gameState.isGameOver = true;
            winText.text = "You died!";
            winText.gameObject.SetActive(true);
        }

        if (gameState.isGameOver)
        {
            player.GetComponent<PlayerController>().gameHasEnded = true;
            // reset player speed
            player.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
            // after 3 seconds, restart the game
            StartCoroutine(RestartGame());
        }

    }

    IEnumerator RestartGame()
    {
        yield return new WaitForSeconds(3);
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }
}
