using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Unity.VisualScripting;
using UnityEngine;

public class EyeTracer : MonoBehaviour
{
    public Color selectedStartColor;
    public Color selectedEndColor;
    public Color unselectedStartColor;
    public Color unselectedEndColor;

    public float startAlpha;
    public float endAlpha;

    private LineRenderer lineRenderer;

    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DrawLine(Vector2 pos1, Vector2 pos2)
    {
        Vector3[] pos = new Vector3[] {pos1, pos2};
        lineRenderer.SetPositions(pos);
    }

    public void DrawLineToCursor(Vector2 pos)
    {
        Vector3[] positions = new Vector3[] {pos, Camera.main.ScreenToWorldPoint(Input.mousePosition) };
        lineRenderer.SetPositions(positions);
    }

    public void SetColor(Color startColor, Color endColor)
    {
        lineRenderer.startColor = new Color(startColor.r, startColor.g, startColor.b, startAlpha);
        lineRenderer.endColor = new Color(endColor.r, endColor.g, endColor.b, endAlpha);
    }

    public void UpdateColor(bool isSelectable)
    {
        if(isSelectable)
        {
            SetColor(selectedStartColor, selectedEndColor);
        }
        else
        {
            SetColor(unselectedStartColor, unselectedEndColor);
        }
    }
}
