using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerScale : MonoBehaviour
{
    [Header("References")]
    public CinemachineVirtualCamera virtualCamera;
    PlayerMovement playerMovement;

    [Header("Player Scale")]
    public float defaultPlayerScale;
    public float playerScaleSpeed;
    [SerializeField] private float maxPlayerScale, minPlayerScale;

    [Header("Ortho Scale")]
    public float defaultOrthoScale;
    public float orthoScaleSpeed;
    [SerializeField] private float maxOrthoScale, minOrthoScale;

    void Start() {
        playerMovement = GetComponent<PlayerMovement>();
    }
    
    void Update()
    {
        if (!playerMovement.IsGrounded()) return;

        if (transform.localScale.x > maxPlayerScale || transform.localScale.x < minPlayerScale) return;

        // Can scale freely if no collision, can only scale down if has collision
        if (IsCollisionFree() || (!IsCollisionFree() && Input.mouseScrollDelta.y < 0f)) {
            
            transform.localScale += Vector3.one * Input.mouseScrollDelta.y * playerScaleSpeed;
            virtualCamera.m_Lens.OrthographicSize = 
                Mathf.Clamp(
                    virtualCamera.m_Lens.OrthographicSize + Input.mouseScrollDelta.y * orthoScaleSpeed, 
                    minOrthoScale, 
                    maxOrthoScale
                );
        }

        ClampScale();
    }

    public bool IsCollisionFree() {
        bool leftFree = !Physics2D.OverlapBox(transform.position + Vector3.left * (transform.localScale.y / 2), new Vector2(0.05f, 0.9f * transform.localScale.y), 0, ~LayerMask.GetMask("Player"));
        bool rightFree = !Physics2D.OverlapBox(transform.position + Vector3.right * (transform.localScale.y / 2), new Vector2(0.05f, 0.9f * transform.localScale.y), 0, ~LayerMask.GetMask("Player"));
        bool topFree = !Physics2D.OverlapBox(transform.position + Vector3.up * (transform.localScale.x / 2), new Vector2(0.9f * transform.localScale.x, 0.05f), 0, ~LayerMask.GetMask("Player"));
    
        return leftFree && rightFree && topFree;
    }

    private void ClampScale() {
        // Clamp player scale
        if (transform.localScale.x > maxPlayerScale) {
            transform.localScale = Vector3.one * maxPlayerScale;
        }

        else if (transform.localScale.x < minPlayerScale) {
            transform.localScale = Vector3.one * minPlayerScale;
        }

        // Clamp camera scale 
        Mathf.Clamp(
            virtualCamera.m_Lens.OrthographicSize, 
            minOrthoScale, 
            maxOrthoScale
        );

    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.white;
        Gizmos.DrawCube(transform.position + Vector3.left * (transform.localScale.x / 2), new Vector2(0.05f, 0.9f * transform.localScale.y));
        Gizmos.DrawCube(transform.position + Vector3.right * (transform.localScale.x / 2), new Vector2(0.05f, 0.9f * transform.localScale.y));
        Gizmos.DrawCube(transform.position + Vector3.up * (transform.localScale.x / 2), new Vector2(0.9f * transform.localScale.x, 0.05f));
    }
}
