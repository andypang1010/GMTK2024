using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate : MonoBehaviour
{
    public GameObject unopenedGate;
    public GameObject openedGate;

    private bool isOpened;

    // Start is called before the first frame update
    void Start()
    {
        openedGate.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Open()
    {
        unopenedGate.SetActive(false);
        openedGate.SetActive(true);

        isOpened = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && isOpened)
        {
            RootManager.Instance.GoGame();
        }
    }
}
