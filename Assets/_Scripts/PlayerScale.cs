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

    [Header("Tagging Objects")]
    public GameObject player;
    public GameObject activeTaggedObject;
    public GameObject playerEyes;
    public LayerMask canSeeThroughLayer;

    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
    }

    void Update()
    {
        if (activeTaggedObject)
        {
            if (!checkIfObjectIsAvailable(activeTaggedObject))
            {
                deselectObject();
            }
        }
        // detect if player's mouse is clicking on a game object
        GameObject clickedObject = Input.GetMouseButtonDown(0) ? GetClickedObject() : null;
        if (clickedObject)
        {
            bool isAvailable = checkIfObjectIsAvailable(clickedObject);
            if (isAvailable)
            {
                Debug.Log("Object is available");
                selectObject(clickedObject);
            }
        }

        GameObject rightClickedObject = Input.GetMouseButtonDown(1) ? GetClickedObject() : null;
        if (rightClickedObject)
        {
            if (rightClickedObject == activeTaggedObject)
            {
                deselectObject();
            }
        }


        if (!playerMovement.IsGrounded()) return;

        if (Mathf.Abs(transform.localScale.x) > maxPlayerScale || Mathf.Abs(transform.localScale.x) < minPlayerScale)
        {
            Debug.Log("Player scale is at max or min");
            return;
        }

        // Can scale freely if no collision, can only scale down if has collision
        if (IsCollisionFree() || (!IsCollisionFree() && Input.mouseScrollDelta.y < 0f))
        {
            if (transform.localScale.x >= 0)
            {
                transform.localScale += Vector3.one * Input.mouseScrollDelta.y * playerScaleSpeed;
            }
            else
            {
                transform.localScale += new Vector3(-1, 1, 1) * Input.mouseScrollDelta.y * playerScaleSpeed;
            }

            virtualCamera.m_Lens.OrthographicSize =
                Mathf.Clamp(
                    virtualCamera.m_Lens.OrthographicSize + Input.mouseScrollDelta.y * orthoScaleSpeed,
                    minOrthoScale,
                    maxOrthoScale
                );

            if (activeTaggedObject)
            {
                if (activeTaggedObject.GetComponent<Scalable>().isScalable() || (Input.mouseScrollDelta.y < 0f))
                {
                    Debug.Log("We can scale the object");
                    if (activeTaggedObject.transform.localScale.x >= 0)
                    {
                        activeTaggedObject.transform.localScale += Vector3.one * Input.mouseScrollDelta.y * playerScaleSpeed;
                    }
                    else
                    {
                        activeTaggedObject.transform.localScale += new Vector3(-1, 1, 1) * Input.mouseScrollDelta.y * playerScaleSpeed;

                    }
                    if (activeTaggedObject.GetComponent<Scalable>().maxScale < activeTaggedObject.transform.localScale.x)
                    {
                        activeTaggedObject.transform.localScale = Vector3.Scale(activeTaggedObject.transform.localScale.x > 0 ? Vector3.one : new Vector3(-1, 1, 1), activeTaggedObject.GetComponent<Scalable>().calculatedMaxScale);
                    }
                    if (activeTaggedObject.GetComponent<Scalable>().minScale > activeTaggedObject.transform.localScale.x)
                    {
                        activeTaggedObject.transform.localScale = Vector3.Scale(activeTaggedObject.transform.localScale.x > 0 ? Vector3.one : new Vector3(-1, 1, 1), activeTaggedObject.GetComponent<Scalable>().calculatedMinScale);
                    }
                }
            }
        }

        ClampScale();
    }

    private GameObject GetClickedObject()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity);

        if (hit.collider != null)
        {
            Debug.Log("[GetClickedObject] hit: " + hit.collider.gameObject.name);
            return hit.collider.gameObject;
        }

        return null;
    }

    public bool checkIfObjectIsAvailable(GameObject clickedObject)
    {
        // check if object has the "Scalable" script
        if (!clickedObject.GetComponent<Scalable>())
        {
            return false;
        }

        // check if object is in front of player
        Vector2 playerFrontNormal = player.transform.localScale.x > 0 ? Vector2.right : Vector2.left;
        Vector2 playerToClickedObject = clickedObject.transform.position - player.transform.position;
        bool isObjectInFront = Vector2.Dot(playerFrontNormal, playerToClickedObject) > 0;
        if (!isObjectInFront) return false;

        // Debug.Log("Current Active Object IS in front of player");

        // raycast from player's eyes to clicked object, disregard canSeeThroughLayer
        RaycastHit2D hit = Physics2D.Raycast(playerEyes.transform.position, clickedObject.transform.position - playerEyes.transform.position, Mathf.Infinity, ~canSeeThroughLayer);
        if (hit.collider != null)
        {

            if (hit.collider.gameObject == clickedObject)
            {
                // Debug.Log("Current Active Object IS visible");
                return true;
            }
        }
        return false;
    }

    public void deselectObject()
    {
        activeTaggedObject.GetComponent<Renderer>().material.color = Color.white;
        activeTaggedObject = null;
    }

    public void selectObject(GameObject clickedObject)
    {
        if (activeTaggedObject)
        {
            deselectObject();
        }
        activeTaggedObject = clickedObject;
        activeTaggedObject.GetComponent<Renderer>().material.color = Color.red;
    }

    public bool IsCollisionFree()
    {
        bool leftFree = !Physics2D.OverlapBox(transform.position + Vector3.left * (Math.Abs(transform.localScale.x) / 2), new Vector2(0.05f, 0.9f * transform.localScale.y), 0, ~LayerMask.GetMask("Player"));
        bool rightFree = !Physics2D.OverlapBox(transform.position + Vector3.right * (Math.Abs(transform.localScale.x) / 2), new Vector2(0.05f, 0.9f * transform.localScale.y), 0, ~LayerMask.GetMask("Player"));
        bool topFree = !Physics2D.OverlapBox(transform.position + Vector3.up * (Math.Abs(transform.localScale.y) / 2), new Vector2(0.9f * Math.Abs(transform.localScale.x), 0.05f), 0, ~LayerMask.GetMask("Player"));

        return leftFree && rightFree && topFree;
    }

    private void ClampScale()
    {
        // Clamp player scale
        if (Math.Abs(transform.localScale.x) > maxPlayerScale)
        {
            transform.localScale = maxPlayerScale * (transform.localScale.x > 0 ? new Vector3(1, 1, 1) : new Vector3(-1, 1, 1));
        }

        else if (Math.Abs(transform.localScale.x) < minPlayerScale)
        {
            transform.localScale = minPlayerScale * (transform.localScale.x > 0 ? new Vector3(1, 1, 1) : new Vector3(-1, 1, 1));
        }

        // Clamp camera scale 
        Mathf.Clamp(
            virtualCamera.m_Lens.OrthographicSize,
            minOrthoScale,
            maxOrthoScale
        );

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawCube(transform.position + Vector3.left * (transform.localScale.x / 2), new Vector2(0.05f, 0.9f * transform.localScale.y));
        Gizmos.DrawCube(transform.position + Vector3.right * (transform.localScale.x / 2), new Vector2(0.05f, 0.9f * transform.localScale.y));
        Gizmos.DrawCube(transform.position + Vector3.up * (transform.localScale.x / 2), new Vector2(0.9f * transform.localScale.x, 0.05f));

        Vector2 playerFrontNormal = player.transform.localScale.x > 0 ? Vector2.right : Vector2.left;
        // gizmo draw player front normal
        Gizmos.color = Color.red;
        Gizmos.DrawLine(player.transform.position, player.transform.position + (Vector3)playerFrontNormal);
    }
}
