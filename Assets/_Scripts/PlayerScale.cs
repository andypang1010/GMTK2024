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
    private float masterScalingSpeed = 1;
    public float scrollScaleSpeed;
    public float keyboardScaleSpeed;
    public float mouseScaleSpeed;
    [SerializeField] private float maxPlayerScale, minPlayerScale;
    public bool playerIsScaling;

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

    [Header("SFX")]
    public AudioClip[] taggingSFXs;
    public AudioClip scaleUpSFX;
    public AudioClip scaleDownSFX;
    public AudioSource scaleSFXSource;
    public float scaleSFXVolume = 0.3f;
    public float taggingSFXVolume = 0.6f;

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
        UpdateSelectedObject();
        UpdateScaling();

        UpdateEyeTracer();

        #region Play Scaling SFX
        if (GetScalingAxis() > 0f)
        {
            scaleSFXSource.clip = scaleUpSFX;
            scaleSFXSource.PlayOneShot(scaleSFXSource.clip, scaleSFXVolume);
        }
        else if (GetScalingAxis() < 0f)
        {
            scaleSFXSource.clip = scaleDownSFX;
            scaleSFXSource.PlayOneShot(scaleSFXSource.clip, scaleSFXVolume);
        }
        #endregion
#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(1))
        {
            EditorGUIUtility.SetWantsMouseJumping(1);
        }
        if (Input.GetMouseButtonUp(1))
        {
            EditorGUIUtility.SetWantsMouseJumping(0);
        }
#endif
    }

    private void UpdateSelectedObject()
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

                // Play tagging SFX
                PlayRandomTaggingSFX();
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
    }

    private void UpdateScaling()
    {
        #region CHECK SCALE CONDITION

        if (!playerMovement.IsGrounded() || Mathf.Abs(playerMovement.horizontal) > 0) return;

        if (Mathf.Abs(transform.localScale.x) > calculatedPlayerMaxScale.x || Mathf.Abs(transform.localScale.x) < calculatedPlayerMinScale.x)
        {
            Debug.Log("Player scale is at max or min");
            return;
        }

        #endregion

        // Can scale freely if no collision, can only scale down if has collision
        if (GetScalingAxis() != 0f && (IsCollisionFree() || GetScalingAxis() < 0f))
        {
            #region SCALE SELF

            Vector3 normalizedPlayerScale = new Vector3(Math.Abs(originalPlayerScale.x), originalPlayerScale.y, originalPlayerScale.z).normalized;
            normalizedPlayerScale.x *= Mathf.Sign(transform.localScale.x);
            transform.localScale += normalizedPlayerScale * GetScalingAxis();

            // push player down when scaling up
            // GetComponent<Rigidbody2D>().AddForce(Vector2.down * 10f, ForceMode2D.Impulse);

            // update playerIsScaling
            playerIsScaling = GetScalingAxis() != 0;

            // Clamp player scale
            if (Math.Abs(transform.localScale.x) > calculatedPlayerMaxScale.x)
            {
                transform.localScale = VecToPlayerFacingDir(calculatedPlayerMaxScale);
            }

            else if (Math.Abs(transform.localScale.x) < calculatedPlayerMinScale.x)
            {
                transform.localScale = VecToPlayerFacingDir(calculatedPlayerMinScale);
            }

            UpdateCamera();
            #endregion
        }

        #region SCALE TAGGED OBJECT
        if (activeTaggedObject)
        {
            Scalable scalableObject = activeTaggedObject.GetComponent<Scalable>();

            if (scalableObject.isScalable() || GetScalingAxis() < 0f)
            {

                // Scale Proportionally
                switch (scalableObject.scaleOption)
                {
                    case ScaleOption.PROPORTIONAL:
                        Vector3 objectOriginalScale = scalableObject.originalScale;

                        // normalize the original scale using the length
                        Vector3 normalizedOriginalScale = new Vector3(Math.Abs(objectOriginalScale.x), objectOriginalScale.y, objectOriginalScale.z).normalized;

                        // scale the active object
                        activeTaggedObject.transform.localScale += VecToActiveObjFacingDir(normalizedOriginalScale) * GetScalingAxis();

                        if (VecToActiveObjFacingDir(normalizedOriginalScale) * GetScalingAxis() != Vector3.zero)
                        {
                            // Debug.Log("Scaling " + activeTaggedObject.name + " proportionally: " + VecToActiveObjFacingDir(normalizedOriginalScale) * GetScalingAxis());
                        }

                        // clamp active object scale
                        if (scalableObject.calculatedMaxScale.x < Mathf.Abs(activeTaggedObject.transform.localScale.x))
                        {
                            // Debug.Log("Clamping object scale - max");
                            activeTaggedObject.transform.localScale = VecToActiveObjFacingDir(scalableObject.calculatedMaxScale);
                        }

                        if (scalableObject.calculatedMinScale.x > Mathf.Abs(activeTaggedObject.transform.localScale.x))
                        {
                            // Debug.Log("Clamping object scale - min");
                            activeTaggedObject.transform.localScale = VecToActiveObjFacingDir(scalableObject.calculatedMinScale);
                        }

                        break;
                    case ScaleOption.VERTICAL:
                        activeTaggedObject.transform.localScale += Vector3.up * GetScalingAxis();

                        // clamp active object scale
                        if (scalableObject.calculatedMaxScale.y < Mathf.Abs(activeTaggedObject.transform.localScale.y))
                        {
                            Debug.Log("Clamping object scale - max");
                            activeTaggedObject.transform.localScale = VecToActiveObjFacingDir(scalableObject.calculatedMaxScale);
                        }

                        if (scalableObject.calculatedMinScale.y > Mathf.Abs(activeTaggedObject.transform.localScale.y))
                        {
                            Debug.Log("Clamping object scale - min");
                            activeTaggedObject.transform.localScale = VecToActiveObjFacingDir(scalableObject.calculatedMinScale);
                        }

                        break;
                }
            }
        }
        #endregion
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

        if (clickedObject == gameObject)
        {
            return;
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
        bool leftFree = !Physics2D.OverlapBox(GetObjEdgePos(-1, 0), GetVerticalBoxSize(), 0, ~LayerMask.GetMask("Player", "UI"));
        bool rightFree = !Physics2D.OverlapBox(GetObjEdgePos(1, 0), GetVerticalBoxSize(), 0, ~LayerMask.GetMask("Player", "UI"));

        bool topFree = !Physics2D.OverlapBox(GetObjEdgePos(0, 1), GetHorizontalBoxSize(), 0, ~LayerMask.GetMask("Player", "UI"));

        return (leftFree || rightFree) && topFree;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawCube(GetObjEdgePos(-1, 0), GetVerticalBoxSize());
        Gizmos.DrawCube(GetObjEdgePos(1, 0), GetVerticalBoxSize());
        Gizmos.DrawCube(GetObjEdgePos(0, 1), GetHorizontalBoxSize());

        Vector2 playerFrontNormal = transform.localScale.x > 0 ? Vector2.right : Vector2.left;
        // gizmo draw player front normal
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + (Vector3)playerFrontNormal);
    }

    private Vector2 GetVerticalBoxSize()
    {
        return new Vector2(0.05f, 0.7f * transform.localScale.y);
    }

    private Vector2 GetHorizontalBoxSize()
    {
        return new Vector2(0.7f * Mathf.Abs(transform.localScale.x), 0.05f);
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

    public void ResetPlayerScale()
    {
        transform.localScale = originalPlayerScale;
        virtualCamera.m_Lens.OrthographicSize = defaultOrthoScale;
    }

    private Vector2 GetWorldSize()
    {
        return GetComponent<Collider2D>().bounds.size;
    }

    public void PlayRandomTaggingSFX()
    {
        scaleSFXSource.clip = taggingSFXs[UnityEngine.Random.Range(0, taggingSFXs.Length)];
        scaleSFXSource.PlayOneShot(scaleSFXSource.clip, taggingSFXVolume);
    }

    /// <summary>
    /// Modify the given vector so the x component is congruent with the player's facing direction
    /// </summary>
    /// <param name="vector"></param>
    /// <returns></returns>
    private Vector3 VecToPlayerFacingDir(Vector3 vector)
    {
        vector.x = Mathf.Sign(transform.localScale.x) * Mathf.Abs(vector.x);
        return vector;
    }

    /// <summary>
    /// Modify the given vector so the x component is congruent with the active object's facing direction
    /// </summary>
    /// <param name="vector"></param>
    /// <returns></returns>
    private Vector3 VecToActiveObjFacingDir(Vector3 vector)
    {
        vector.x = Mathf.Sign(activeTaggedObject.transform.localScale.x) * Mathf.Abs(vector.x);
        return vector;
    }

    private float GetScalingAxis()
    {
        float axis = 0;
        if (Input.mouseScrollDelta.y != 0)
        {
            axis = Input.mouseScrollDelta.y * scrollScaleSpeed;
        }
        if (Input.GetAxisRaw("Vertical") != 0)
        {
            axis = Input.GetAxisRaw("Vertical") * keyboardScaleSpeed;
        }
        if (Input.GetMouseButton(1))
        {
            axis = Input.mousePositionDelta.y * mouseScaleSpeed;

        }
        return axis * masterScalingSpeed;
    }

    public void SetMasterScaleingSpeed(float scalingSpeed)
    {
        masterScalingSpeed = scalingSpeed;
    }
}
