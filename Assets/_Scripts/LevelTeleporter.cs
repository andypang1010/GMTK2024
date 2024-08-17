using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelTeleporter : MonoBehaviour
{
    public string levelName;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            RootManager.Instance.GoToScene(levelName);
        }
    }
}
