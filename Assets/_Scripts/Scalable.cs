using System;
using UnityEngine;

public class Scalable : MonoBehaviour
{
    public float maxScale = 5f;
    public float minScale = 0.5f;
    public ScaleOption scaleOption;
    [HideInInspector] public Vector3 originalScale;
    [HideInInspector] public Vector3 calculatedMinScale;
    [HideInInspector] public Vector3 calculatedMaxScale;
    private LayerMask whatToIgnore;

    void Start()
    {
        originalScale = transform.localScale;
        whatToIgnore = LayerMask.GetMask("Player", "UI", "Scalable");

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
        print(gameObject.name + " isScalable: " + isScalable());
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
                    return false;
                }

                if (transform.localScale.y > calculatedMaxScale.y
                || transform.localScale.y < calculatedMinScale.y)
                {
                    return false;
                }

                return true;

            case ScaleOption.VERTICAL:
                if (transform.localScale.y > calculatedMaxScale.y
                || transform.localScale.y < calculatedMinScale.y)
                {
                    return false;
                }

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

        return leftFree && rightFree && topFree;
    }

    public void ResetScale()
    {
        transform.localScale = originalScale;
    }

}

public enum ScaleOption
{
    PROPORTIONAL,
    VERTICAL
}