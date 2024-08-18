using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class Elevator : MonoBehaviour
{
    public float moveSpeed;
    public Transform[] points;
    public int startIndex;
    private int index;
    
    private void Start() {
        index = startIndex;
    }

    private void Update() {
        if (Vector2.Distance(transform.position, points[index].position) < 0.02f) {
            index++;

            index %= points.Length;
        }

        transform.position = Vector2.MoveTowards(transform.position, points[index].position, moveSpeed * Time.deltaTime);
    }
}
