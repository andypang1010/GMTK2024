using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Elevator : MonoBehaviour
{
    public bool isActive;
    public MoveType moveType;
    public float moveSpeed;
    public Transform[] points;
    public int startIndex;
    private Vector3 startPosition;
    private int index;
    
    private void Start() {
        index = startIndex;
        startPosition = transform.position;
    }

    private void Update() {
        if (!isActive) return;

        if (moveType == MoveType.OSCILLATE) {
            if (Vector2.Distance(transform.position, points[index].position) < 0.02f) {
                index++;

                index %= points.Length;
            }

            transform.position = 
                Vector2.MoveTowards(transform.position, points[index].position, moveSpeed * Time.deltaTime);
        }

        else if (moveType == MoveType.SINGLE) {
            if (Vector2.Distance(transform.position, points[0].position) > 0.02f)
            transform.position = 
                Vector2.MoveTowards(transform.position, points[0].position, moveSpeed * Time.deltaTime);

        }

    }

    public void ResetElevator() {
        transform.position = startPosition;
    }

    public void ActivateElevator() {
        isActive = true;
    }

    public enum MoveType {
        OSCILLATE,
        SINGLE
    }
}
