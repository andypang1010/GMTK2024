using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelTeleporter : MonoBehaviour
{
    public int toLevelIndex;
    public int fromLevelIndex;
    public Transform toPosition;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            RootManager.Instance.GoToScene(toLevelIndex);
            // GameManager.Instance.PlacePlayerInLevel(toLevelIndex, fromLevelIndex, toPosition);
        }
    }
}
