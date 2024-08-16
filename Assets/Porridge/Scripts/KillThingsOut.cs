using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillThingsOut : MonoBehaviour
{
    public bool shouldRestart = false;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            shouldRestart = true;
            return;
        }
        Destroy(collision.gameObject);
    }
}
