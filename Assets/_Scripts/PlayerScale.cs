using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class PlayerScale : MonoBehaviour
{
    [Header("References")]
    public CinemachineVirtualCamera virtualCamera;
    PlayerMovement playerMovement;
    public EyeTracer eyeTracer;

    [Header("Player Scale")]
    public float defaultPlayerScale;
    public float playerScaleSpeed;
    [SerializeField] private float maxPlayerScale, minPlayerScale;

    [Header("Ortho Scale")]
    public float defaultOrthoScale;
    public float camHeightAtMinScale, camHeightAtMaxScale;
    [SerializeField] private float maxOrthoScale, minOrthoScale;
    private CinemachineFramingTransposer cinemachineTransposer;
    private float initCamOffsetPercentage;

    [Header("Tagging Objects")]
    public GameObject activeTaggedObject;
    public GameObject playerEyes;
    public LayerMask canSeeThroughLayer;
    public LayerMask canSeeThroughWhenTagged;
    [HideInInspector] private Vector3 originalPlayerScale;
    [HideInInspector] public Vector3 calculatedPlayerMaxScale;
    [HideInInspector] public Vector3 calculatedPlayerMinScale;

    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        originalPlayerScale = transform.localScale;
        calculatedPlayerMaxScale = maxPlayerScale * originalPlayerScale;
        calculatedPlayerMinScale = minPlayerScale * originalPlayerScale;

        cinemachineTransposer = virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
        initCamOffsetPercentage = cinemachineTransposer.m_TrackedObjectOffset.y / GetWorldSize().y;
    }

    void Update()
    {
        if (activeTaggedObject)
        {
            if (!IsObjectAvailable(activeTaggedObject, true))
            {
                DeselectObject();
            }
        }

        // detect if player's mouse is clicking on a game object
        GameObject clickedObject = Input.GetMouseButtonDown(0) ? GetClickedObject() : null;
        if (clickedObject)
        {
            bool isAvailable = IsObjectAvailable(clickedObject);
            if (isAvailable)
            {
                // Debug.Log("Object is available");
                SelectObject(clickedObject);
            }
        }

        // Deselect if clicking somewhere else
        if (Input.GetMouseButtonDown(0) && activeTaggedObject && (clickedObject == null || !IsObjectAvailable(clickedObject)))
        {
            DeselectObject();
        }

        GameObject rightClickedObject = Input.GetMouseButtonDown(1) ? GetClickedObject() : null;
        if (rightClickedObject)
        {
            if (rightClickedObject == activeTaggedObject)
            {
                DeselectObject();
            }
        }

        UpdateEyeTracer();

        #region CHECK SCALE CONDITION

        if (!playerMovement.IsGrounded() || Mathf.Abs(playerMovement.horizontal) > 0) return;

        if (Mathf.Abs(transform.localScale.x) > calculatedPlayerMaxScale.x || Mathf.Abs(transform.localScale.x) < calculatedPlayerMinScale.x)
        {
            Debug.Log("Player scale is at max or min");
            return;
        }

        #endregion

        // Can scale freely if no collision, can only scale down if has collision
        if (IsCollisionFree() || (!IsCollisionFree() && Input.mouseScrollDelta.y < 0f))
        {
            #region SCALE SELF

            Vector3 normalizedPlayerScale = new Vector3(Math.Abs(originalPlayerScale.x), originalPlayerScale.y, originalPlayerScale.z).normalized;
            transform.localScale += Vector3.Scale(new Vector3(Mathf.Sign(transform.localScale.x), 1, 1), normalizedPlayerScale) * Input.mouseScrollDelta.y * playerScaleSpeed;

            // Clamp player scale
            if (Math.Abs(transform.localScale.x) > calculatedPlayerMaxScale.x)
            {
                transform.localScale = Vector3.Scale(calculatedPlayerMaxScale, new Vector3(Mathf.Sign(transform.localScale.x), 1, 1));
            }

            else if (Math.Abs(transform.localScale.x) < calculatedPlayerMinScale.x)
            {
                transform.localScale = Vector3.Scale(calculatedPlayerMinScale, new Vector3(Mathf.Sign(transform.localScale.x), 1, 1));
            }

            UpdateCamera();
            #endregion
        }

        #region SCALE TAGGED OBJECT
        if (activeTaggedObject)
        {
            Scalable scalableObject = activeTaggedObject.GetComponent<Scalable>();

            if (scalableObject.isScalable() || (Input.mouseScrollDelta.y < 0f))
            {

                // Scale Proportionally
                switch (scalableObject.scaleOption)
                {
                    case ScaleOption.PROPORTIONAL:
                        Vector3 objectOriginalScale = scalableObject.originalScale;

                        // normalize the original scale using the length
                        Vector3 normalizedOriginalScale = new Vector3(Math.Abs(objectOriginalScale.x), objectOriginalScale.y, objectOriginalScale.z).normalized;

                        // scale the active object
                        activeTaggedObject.transform.localScale += Vector3.Scale(new Vector3(Mathf.Sign(activeTaggedObject.transform.localScale.x), 1, 1), normalizedOriginalScale) * Input.mouseScrollDelta.y * playerScaleSpeed;

                        if (Vector3.Scale(new Vector3(Mathf.Sign(activeTaggedObject.transform.localScale.x), 1, 1), normalizedOriginalScale) * Input.mouseScrollDelta.y * playerScaleSpeed != Vector3.zero)
                        {
                            Debug.Log("Scaling " + activeTaggedObject.name + " proportionally: " + Vector3.Scale(new Vector3(Mathf.Sign(activeTaggedObject.transform.localScale.x), 1, 1), normalizedOriginalScale) * Input.mouseScrollDelta.y * playerScaleSpeed);
                        }

                        // clamp active object scale
                        if (scalableObject.calculatedMaxScale.x < Mathf.Abs(activeTaggedObject.transform.localScale.x))
                        {
                            Debug.Log("Clamping object scale - max");
                            activeTaggedObject.transform.localScale = Vector3.Scale(activeTaggedObject.transform.localScale.x > 0 ? Vector3.one : new Vector3(-1, 1, 1), scalableObject.calculatedMaxScale);
                        }

                        if (scalableObject.calculatedMinScale.x > Mathf.Abs(activeTaggedObject.transform.localScale.x))
                        {
                            Debug.Log("Clamping object scale - min");
                            activeTaggedObject.transform.localScale = Vector3.Scale(activeTaggedObject.transform.localScale.x > 0 ? Vector3.one : new Vector3(-1, 1, 1), scalableObject.calculatedMinScale);
                        }

                        break;
                    case ScaleOption.VERTICAL:
                        activeTaggedObject.transform.localScale += Vector3.up * Input.mouseScrollDelta.y * playerScaleSpeed;

                        // clamp active object scale
                        if (scalableObject.calculatedMaxScale.y < Mathf.Abs(activeTaggedObject.transform.localScale.y))
                        {
                            Debug.Log("Clamping object scale - max");
                            activeTaggedObject.transform.localScale = Vector3.Scale(activeTaggedObject.transform.localScale.y > 0 ? Vector3.one : new Vector3(-1, 1, 1), scalableObject.calculatedMaxScale);
                        }

                        if (scalableObject.calculatedMinScale.y > Mathf.Abs(activeTaggedObject.transform.localScale.y))
                        {
                            Debug.Log("Clamping object scale - min");
                            activeTaggedObject.transform.localScale = Vector3.Scale(activeTaggedObject.transform.localScale.y > 0 ? Vector3.one : new Vector3(-1, 1, 1), scalableObject.calculatedMinScale);
                        }

                        break;
                }

            }
        }

        #endregion

    }

    private void FixedUpdate()
    {
        //UpdateCamera();
    }

    private void LateUpdate()
    {
        //UpdateCamera();
    }

    private GameObject GetClickedObject()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity, ~LayerMask.GetMask("UI"));

        if (hit.collider != null)
        {
            Debug.Log("[GetClickedObject] hit: " + hit.collider.gameObject.name);
            return hit.collider.gameObject;
        }

        return null;
    }

    public bool IsObjectAvailable(GameObject clickedObject, bool checkForDeselect = false)
    {
        // check if object has the "Scalable" script
        if (!clickedObject.GetComponent<Scalable>())
        {
            return false;
        }

        // check if object is in front of player
        //Vector2 playerFrontNormal = transform.localScale.x > 0 ? Vector2.right : Vector2.left;
        //Vector2 playerToClickedObject = clickedObject.transform.position - transform.position;
        //bool isObjectInFront = Vector2.Dot(playerFrontNormal, playerToClickedObject) >= 0;
        //if (!isObjectInFront) return false;

        // Debug.Log("Current Active Object IS in front of player");

        // raycast from player's eyes to clicked object, disregard canSeeThroughLayer
        LayerMask combineMask;
        if (checkForDeselect)
        {
            combineMask = canSeeThroughLayer | canSeeThroughWhenTagged;
        }
        else
        {
            combineMask = canSeeThroughLayer;
        }
        RaycastHit2D hit = Physics2D.Raycast(playerEyes.transform.position, clickedObject.transform.position - playerEyes.transform.position, Mathf.Infinity, ~combineMask);

        Debug.Log("Hit: " + hit.collider.name);

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

    private void UpdateEyeTracer()
    {
        Vector2 eyePos = playerEyes.transform.position;
        Vector2 destPos;
        if (activeTaggedObject != null)
        {
            destPos = activeTaggedObject.transform.position;
        }
        else
        {
            Vector2 cursorPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 posDiff = cursorPos - eyePos;
            RaycastHit2D hit = Physics2D.Raycast(eyePos, posDiff, posDiff.magnitude, ~canSeeThroughLayer);

            eyeTracer.UpdateColor(hit && hit.collider.gameObject.GetComponent<Scalable>());

            destPos = hit ? hit.point : cursorPos;
        }
        eyeTracer.DrawLine(eyePos, destPos);
    }

    private void UpdateCamera()
    {
        float camHeight = Mathf.Lerp(camHeightAtMinScale, camHeightAtMaxScale, GetPercentageScale());

        virtualCamera.m_Lens.OrthographicSize =
                    Mathf.Clamp(
                        camHeight,
                        minOrthoScale,
                        maxOrthoScale
                    );

        cinemachineTransposer.m_TrackedObjectOffset = new Vector3(0, GetWorldSize().y * initCamOffsetPercentage);
    }

    /// <summary>
    /// Return the current scale as a percentage of the max and min scale
    /// </summary>
    /// <returns>value between 0 to 1</returns>
    private float GetPercentageScale()
    {
        float maxMinDifference = calculatedPlayerMaxScale.x - calculatedPlayerMinScale.x;
        float playerCurrScale = Math.Abs(transform.localScale.x) - calculatedPlayerMinScale.x;
        return playerCurrScale / maxMinDifference;
    }

    public void DeselectObject()
    {
        Debug.Log("Deselecting object: " + activeTaggedObject.name);
        Color objectColor = activeTaggedObject.GetComponentInChildren<SpriteRenderer>().color;
        activeTaggedObject.GetComponentInChildren<SpriteRenderer>().color = new Color(objectColor.r, objectColor.g, objectColor.b, 100);

        activeTaggedObject = null;
    }

    public void SelectObject(GameObject clickedObject)
    {
        if (activeTaggedObject)
        {
            DeselectObject();
        }

        GameObject realScalableObj = clickedObject.GetComponent<Scalable>().realScalableObj;
        if (realScalableObj != null)
        {
            activeTaggedObject = realScalableObj;
        }
        else
        {
            activeTaggedObject = clickedObject;
        }

        Color objectColor = activeTaggedObject.GetComponentInChildren<SpriteRenderer>().color;
        activeTaggedObject.GetComponentInChildren<SpriteRenderer>().color = new Color(objectColor.r, objectColor.g, objectColor.b, 255);

        Debug.Log("Selecting object: " + activeTaggedObject.name);
    }

    public bool IsCollisionFree()
    {
        bool leftFree = !Physics2D.OverlapBox(transform.position + Vector3.left * (Math.Abs(transform.localScale.x) / 2), new Vector2(0.05f, 0.9f * transform.localScale.y), 0, ~LayerMask.GetMask("Player", "UI"));
        // print("Left: " + Physics2D.OverlapBox(transform.position + Vector3.left * (Math.Abs(transform.localScale.x) / 2), new Vector2(0.05f, 0.9f * transform.localScale.y), 0, ~LayerMask.GetMask("Player", "UI")).gameObject.name);

        bool rightFree = !Physics2D.OverlapBox(transform.position + Vector3.right * (Math.Abs(transform.localScale.x) / 2), new Vector2(0.05f, 0.9f * transform.localScale.y), 0, ~LayerMask.GetMask("Player", "UI"));
        // print("Right: " + Physics2D.OverlapBox(transform.position + Vector3.right * (Math.Abs(transform.localScale.x) / 2), new Vector2(0.05f, 0.9f * transform.localScale.y), 0, ~LayerMask.GetMask("Player", "UI")).gameObject.name);

        bool topFree = !Physics2D.OverlapBox(transform.position + Vector3.up * (transform.localScale.y / 2), new Vector2(0.9f * Math.Abs(transform.localScale.x), 0.05f), 0, ~LayerMask.GetMask("Player", "UI"));
        // print("Top: " + Physics2D.OverlapBox(transform.position + Vector3.up * (transform.localScale.y / 2), new Vector2(0.9f * Math.Abs(transform.localScale.x), 0.05f), 0, ~LayerMask.GetMask("Player", "UI")).gameObject.name);


        return leftFree && rightFree && topFree;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawCube(transform.position + Vector3.left * (Mathf.Abs(transform.localScale.x) / 2), new Vector2(0.05f, 0.9f * transform.localScale.y));
        Gizmos.DrawCube(transform.position + Vector3.right * (Mathf.Abs(transform.localScale.x) / 2), new Vector2(0.05f, 0.9f * transform.localScale.y));
        Gizmos.DrawCube(transform.position + Vector3.up * (transform.localScale.y / 2), new Vector2(0.9f * Mathf.Abs(transform.localScale.x), 0.05f));

        Vector2 playerFrontNormal = transform.localScale.x > 0 ? Vector2.right : Vector2.left;
        // gizmo draw player front normal
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + (Vector3)playerFrontNormal);
    }

    public void ResetPlayerScale()
    {
        transform.localScale = originalPlayerScale;
        virtualCamera.m_Lens.OrthographicSize = defaultOrthoScale;
    }

    private Vector2 GetWorldSize()
    {
        return GetComponent<Collider2D>().bounds.size;
    }
}
