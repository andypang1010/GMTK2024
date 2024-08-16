using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIBrainDetection : MonoBehaviour
{
    public bool isPlayerDetected = false;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter2D(Collider2D hitInfo)
    {
        if (hitInfo.tag == "Player")
        {
            Debug.Log("Player Detected");
            isPlayerDetected = true;
        }
    }

    void OnTriggerExit2D(Collider2D hitInfo)
    {
        if (hitInfo.tag == "Player")
        {
            Debug.Log("Player Left");
            isPlayerDetected = false;
        }
    }
}
