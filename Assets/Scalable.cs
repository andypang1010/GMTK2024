using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scalable : MonoBehaviour
{
    public LayerMask whatToIgnore;
    public float maxScale = 5f;
    public float minScale = 0.5f;
    public Vector3 originalScale;
    public Vector3 calculatedMinScale;
    public Vector3 calculatedMaxScale;
    // Start is called before the first frame update
    void Start()
    {
        originalScale = transform.localScale;
        calculatedMinScale = minScale * originalScale;
        calculatedMaxScale = maxScale * originalScale;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public bool isScalable()
    {
        if (transform.localScale.y > calculatedMaxScale.y || transform.localScale.y < calculatedMinScale.y)
        {
            return false;
        }
        if (!IsCollisionFree())
        {
            return false;
        }
        return true;
    }

    public bool IsCollisionFree()
    {
        bool leftFree = !Physics2D.OverlapBox(transform.position + Vector3.left * (Math.Abs(transform.localScale.x) / 2), new Vector2(0.05f, 0.8f * transform.localScale.y), 0, ~whatToIgnore);
        bool rightFree = !Physics2D.OverlapBox(transform.position + Vector3.right * (Math.Abs(transform.localScale.x) / 2), new Vector2(0.05f, 0.8f * transform.localScale.y), 0, ~whatToIgnore);
        bool topFree = !Physics2D.OverlapBox(transform.position + Vector3.up * (Math.Abs(transform.localScale.y) / 2), new Vector2(0.8f * Math.Abs(transform.localScale.x), 0.05f), 0, ~whatToIgnore);

        bool result = leftFree && rightFree && topFree;
        // if (result)
        // {
        //     Debug.Log("Collision free");
        // }
        // else
        // {
        //     Debug.Log("Collision detected" + leftFree + rightFree + topFree);
        // }

        return leftFree && rightFree && topFree;
    }
}
