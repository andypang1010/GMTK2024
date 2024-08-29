using System;
using UnityEngine;

public class Scalable : MonoBehaviour
{
    public float maxScale = 5f;
    public float minScale = 0.5f;
    public ScaleOption scaleOption;
    public LayerMask whatToIgnore;
    public GameObject realScalableObj;
    [HideInInspector] public Vector3 originalScale;
    [HideInInspector] public Vector3 calculatedMinScale;
    [HideInInspector] public Vector3 calculatedMaxScale;

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
        //print(gameObject.name + " isScalable: " + isScalable());
    }

    public bool isScalable()
    {
        //print(gameObject.name + " " + IsCollisionFree());
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
        Collider2D leftCol = Physics2D.OverlapBox(GetObjEdgePos(-1, 0), GetVerticalBoxSize(), 0, ~whatToIgnore);
        Collider2D rightCol = Physics2D.OverlapBox(GetObjEdgePos(1, 0), GetVerticalBoxSize(), 0, ~whatToIgnore);
        Collider2D topCol = Physics2D.OverlapBox(GetObjEdgePos(0, 1), GetHorizontalBoxSize(), 0, ~whatToIgnore);

        //Debug.Log(gameObject.name + " left collider: " + leftCol?.name);
        //Debug.Log(gameObject.name + " right collider: " + rightCol?.name);
        //Debug.Log(gameObject.name + " top collider: " + topCol?.name);

        bool leftFree = !leftCol;
        bool rightFree = !rightCol;
        bool topFree = !topCol;

        bool canScale;
        if (scaleOption == ScaleOption.PROPORTIONAL)
        {
            canScale = (leftFree || rightFree) && topFree;
        }
        else
        {
            canScale = topFree;
        }

        return canScale;
    }

    // Get box size for left and right
    private Vector2 GetVerticalBoxSize()
    {
        return new Vector2(0.05f, 0.7f * transform.localScale.y);
    }

    // Get box size for top
    private Vector2 GetHorizontalBoxSize()
    {
        return new Vector2(0.8f * Math.Abs(transform.localScale.x), 0.05f);
    }

    /// <summary>
    /// Get the position of a given edge's center. One of xDir and yDir should always be 0.
    /// </summary>
    /// <param name="xDir">xDir = 1 for pos X, -1 for neg X</param>
    /// <param name="yDir">yDir = 1 for pos Y, -1 for neg Y</param>
    /// <returns>The box position</returns>
    private Vector2 GetObjEdgePos(int xDir, int yDir)
    {
        Vector3 dir = new Vector3(xDir, yDir, 0);
        float posOffset = 0;
        if (xDir != 0)
        {
            posOffset = Math.Abs(transform.localScale.x) / 2;
        }
        else if (yDir != 0)
        {
            posOffset = Math.Abs(transform.localScale.y) / 2;
        }
        return transform.position + dir * posOffset;
    }

    public void ResetScale()
    {
        transform.localScale = originalScale;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawCube(GetObjEdgePos(-1, 0), GetVerticalBoxSize());
        Gizmos.DrawCube(GetObjEdgePos(1, 0), GetVerticalBoxSize());
        Gizmos.DrawCube(GetObjEdgePos(0, 1), GetHorizontalBoxSize());
    }
}

public enum ScaleOption
{
    PROPORTIONAL,
    VERTICAL
}