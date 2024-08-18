using System;
using UnityEngine;

public class Scalable : MonoBehaviour
{
    public LayerMask whatToIgnore;
    public float maxScale = 5f;
    public float minScale = 0.5f;
    public ScaleOption scaleOption;
    public Vector3 originalScale;
    public Vector3 calculatedMinScale;
    public Vector3 calculatedMaxScale;
    // Start is called before the first frame update
    void Start()
    {
        originalScale = transform.localScale;

        switch (scaleOption)
        {
            case ScaleOption.PROPORTIONAL:
                calculatedMinScale = minScale * originalScale;
                calculatedMaxScale = maxScale * originalScale;
                break;
            case ScaleOption.VERTICAL:
                calculatedMinScale = originalScale;
                calculatedMinScale.y = minScale;

                calculatedMaxScale = originalScale;
                calculatedMaxScale.y = maxScale;

                break;

        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public bool isScalable()
    {
        if (!IsCollisionFree())
        {
            return false;
        }

        switch (scaleOption)
        {
            case ScaleOption.PROPORTIONAL:
                if (Mathf.Abs(transform.localScale.x) > calculatedMaxScale.x
                || Mathf.Abs(transform.localScale.x) < calculatedMinScale.x)
                {
                    Debug.Log("X scale out of bounds");
                    return false;
                }

                if (transform.localScale.y > calculatedMaxScale.y
                || transform.localScale.y < calculatedMinScale.y)
                {
                    Debug.Log("Y scale out of bounds");
                    return false;
                }

                return true;

            case ScaleOption.VERTICAL:
                if (transform.localScale.y > calculatedMaxScale.y
                || transform.localScale.y < calculatedMinScale.y)
                {
                    return false;
                }

                return true;
        }

        return false;
    }

    public bool IsCollisionFree()
    {
        bool leftFree = !Physics2D.OverlapBox(transform.position + Vector3.left * (Math.Abs(transform.localScale.x) / 2), new Vector2(0.05f, 0.8f * transform.localScale.y), 0, ~whatToIgnore);
        bool rightFree = !Physics2D.OverlapBox(transform.position + Vector3.right * (Math.Abs(transform.localScale.x) / 2), new Vector2(0.05f, 0.8f * transform.localScale.y), 0, ~whatToIgnore);
        bool topFree = !Physics2D.OverlapBox(transform.position + Vector3.up * (Math.Abs(transform.localScale.y) / 2), new Vector2(0.8f * Math.Abs(transform.localScale.x), 0.05f), 0, ~whatToIgnore);

        bool result = leftFree && rightFree && topFree;
        if (result)
        {
            // Debug.Log("Collision free");
        }
        else
        {
            Debug.Log("Collision detected" + leftFree + rightFree + topFree);
        }

        return leftFree && rightFree && topFree;
    }
}

public enum ScaleOption
{
    PROPORTIONAL,
    VERTICAL
}