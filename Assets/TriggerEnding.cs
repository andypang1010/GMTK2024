using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerEnding : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            RootManager.Instance.GoEndingCG();
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            RootManager.Instance.GoEndingCG();
        }
    }
}
