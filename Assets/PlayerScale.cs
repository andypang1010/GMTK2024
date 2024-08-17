using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class PlayerScale : MonoBehaviour
{
    [Header("References")]
    public CinemachineVirtualCamera virtualCamera;

    [Header("Player Scale")]
    public float defaultPlayerScale;
    public float playerScaleSpeed;
    [SerializeField] private float maxPlayerScale, minPlayerScale;

    [Header("Ortho Scale")]
    public float defaultOrthoScale;
    public float orthoScaleSpeed;
    [SerializeField] private float maxOrthoScale, minOrthoScale;
    
    void Start()
    {
        transform.localScale = Vector3.one * defaultPlayerScale;
        virtualCamera.m_Lens.OrthographicSize = defaultOrthoScale;
    }

    void Update()
    {

        // Check if currently within min and max scale
        if (transform.localScale.x <= maxPlayerScale
            && transform.localScale.x >= minPlayerScale) {

            // Can scale freely if no collision, can only scale down if has collision
            if (IsCollisionFree() || (!IsCollisionFree() && Input.mouseScrollDelta.y < 0f)) {
                transform.localScale += Vector3.one * Input.mouseScrollDelta.y * playerScaleSpeed * Time.deltaTime;
                virtualCamera.m_Lens.OrthographicSize = Mathf.Clamp(virtualCamera.m_Lens.OrthographicSize + Input.mouseScrollDelta.y * orthoScaleSpeed * Time.deltaTime, minOrthoScale, maxOrthoScale);
            }

            Mathf.Clamp(virtualCamera.m_Lens.OrthographicSize, minOrthoScale, maxOrthoScale);

            // Clip scale to between min and max scale
            if (transform.localScale.x > maxPlayerScale) {
                transform.localScale = Vector3.one * maxPlayerScale;
            }

            else if (transform.localScale.x < minPlayerScale) {
                transform.localScale = Vector3.one * minPlayerScale;
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
