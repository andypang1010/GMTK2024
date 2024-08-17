using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using TreeEditor;
using UnityEngine;

public class PlayerScale : MonoBehaviour
{
    public CinemachineVirtualCamera virtualCamera;
    public float defaultScale, changeSpeed;
    [SerializeField] private float maxScale, minScale;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        // Check if can scale
        if (transform.localScale.x <= maxScale
            && transform.localScale.x >= minScale) {
                
            if (IsCollisionFree() || (!IsCollisionFree() && Input.mouseScrollDelta.y < 0f)) {
                transform.localScale += Vector3.one * Input.mouseScrollDelta.y * changeSpeed * Time.deltaTime;
            }

            // Clip scale to between min and max scale
            if (transform.localScale.x > maxScale) {
                transform.localScale = Vector3.one * maxScale;
            }

            else if (transform.localScale.x < minScale) {
                transform.localScale = Vector3.one * minScale;
            }
        }
    }

    public bool IsCollisionFree() {
        bool leftFree = !Physics2D.OverlapBox(transform.position + Vector3.left * (transform.localScale.y / 2), new Vector2(0.05f, 0.9f * transform.localScale.y), 0, ~LayerMask.GetMask("Player"));
        bool rightFree = !Physics2D.OverlapBox(transform.position + Vector3.right * (transform.localScale.y / 2), new Vector2(0.05f, 0.9f * transform.localScale.y), 0, ~LayerMask.GetMask("Player"));
        bool topFree = !Physics2D.OverlapBox(transform.position + Vector3.up * (transform.localScale.x / 2), new Vector2(0.9f * transform.localScale.x, 0.05f), 0, ~LayerMask.GetMask("Player"));
    
        return leftFree && rightFree && topFree;
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.white;
        Gizmos.DrawCube(transform.position + Vector3.left * (transform.localScale.x / 2), new Vector2(0.05f, 0.9f * transform.localScale.y));
        Gizmos.DrawCube(transform.position + Vector3.right * (transform.localScale.x / 2), new Vector2(0.05f, 0.9f * transform.localScale.y));
        Gizmos.DrawCube(transform.position + Vector3.up * (transform.localScale.x / 2), new Vector2(0.9f * transform.localScale.x, 0.05f));
    }
}
