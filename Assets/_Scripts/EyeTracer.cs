using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Unity.VisualScripting;
using UnityEngine;

public class EyeTracer : MonoBehaviour
{
    public Color selectedColor;

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

    public void SetColor(Color color)
    {
        lineRenderer.startColor = color.WithAlpha(1);
        lineRenderer.endColor = color.WithAlpha(0.2f);
    }

    public void UpdateColor(bool isSelectable)
    {
        if(isSelectable)
        {
            SetColor(selectedColor);
        }
        else
        {
            SetColor(Color.white);
        }
    }
}
