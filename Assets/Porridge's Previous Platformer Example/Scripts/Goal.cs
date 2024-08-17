using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Goal : MonoBehaviour
{

    public bool isGameWon;
    // Start is called before the first frame update
    void Start()
    {
        isGameWon = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("You win!");
            isGameWon = true;
        }
    }
}
