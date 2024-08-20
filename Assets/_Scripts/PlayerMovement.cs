using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask teaLayer;
    private PlayerScale playerScale;
    private Rigidbody2D rb;
    public Animator playerAnim;

    [Header("Movement")]
    [SerializeField] private float moveSpeed;
    [HideInInspector] public float horizontal;

    [Header("Jump")]
    [SerializeField] private float coyoteTime;
    [SerializeField] private float jumpBuffer;
    private float coyoteTimeCounter, jumpBufferCounter;
    private bool previouslyGrounded = true;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerScale = GetComponent<PlayerScale>();
        playerAnim.SetBool("IsWalking", false);
        playerAnim.SetBool("IsJumping", false);
    }

    void Update()
    {
        horizontal = Input.GetAxisRaw("Horizontal");

        if (Mathf.Abs(horizontal) > 0)
        {
            rb.isKinematic = false;
        }

        // Coyote Time Check
        if (IsGrounded())
        {
            coyoteTimeCounter = coyoteTime;

            if (!previouslyGrounded)
            {
                playerAnim.SetBool("IsJumping", false);
            }
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;

            if (previouslyGrounded && coyoteTimeCounter > 0f)
            {
                playerAnim.SetBool("IsJumping", true);
            }
        }

        // Jump Buffer Check
        if (Input.GetKeyDown(KeyCode.Space))
        {
            jumpBufferCounter = jumpBuffer;
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }

        if (coyoteTimeCounter > 0f && jumpBufferCounter > 0f)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                rb.isKinematic = false;
            }

            rb.velocity = new Vector2(rb.velocity.x, HeightToVelocity());

            // playerAnim.SetBool("IsJumping", true);
        }

        if (playerScale.playerIsScaling)
        {
            playerAnim.SetBool("IsScaling", true);
            if (playerScale.activeTaggedObject != null)
            {
                playerAnim.SetFloat("hasActiveObject", 1);
            }
            else
            {
                playerAnim.SetFloat("hasActiveObject", 0);
            }
        }
        else
        {
            playerAnim.SetBool("IsScaling", false);
        }

        Flip();

        previouslyGrounded = IsGrounded();
    }

    void Flip()
    {
        if (horizontal > 0)
        {
            // detect if player's x scale is already positive
            if (transform.localScale.x > 0)
            {
                return;
            }
            else
            {
                Vector3 tempScale = transform.localScale;
                tempScale.x *= -1;
                transform.localScale = tempScale;
            }
        }
        else if (horizontal < 0)
        {
            // detect if player's x scale is already negative
            if (transform.localScale.x < 0)
            {
                return;
            }
            else
            {
                Vector3 tempScale = transform.localScale;
                tempScale.x *= -1;
                transform.localScale = tempScale;
            }
        }
    }

    void FixedUpdate()
    {
        if (IsGrounded())
        {
            rb.velocity = new Vector2(horizontal * moveSpeed, rb.velocity.y);

            if (Mathf.Abs(horizontal) > 0)
            {
                playerAnim.SetBool("IsWalking", true);
            }
            else
            {
                playerAnim.SetBool("IsWalking", false);
            }
        }

        // Damping in air
        else
        {
            rb.velocity = new Vector2(horizontal * moveSpeed * 0.7f, rb.velocity.y);
        }
    }

    public bool IsGrounded()
    {
        return Physics2D.OverlapBox(groundCheck.position, new Vector2(0.75f * Mathf.Abs(transform.localScale.x), 0.05f), 0f, groundLayer);
    }

    float HeightToVelocity()
    {
        return Mathf.Sqrt(2 * (1f * Mathf.Abs(transform.localScale.x)) / Mathf.Abs(Physics2D.gravity.y)) * Mathf.Abs(Physics2D.gravity.y);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawCube(groundCheck.position, new Vector2(0.75f * Mathf.Abs(transform.localScale.x), 0.05f));
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        // If the collider is of a layer in the Tea Layer
        if ((teaLayer.value & 1 << other.gameObject.layer) > 0)
        {

            // Reset position and scale
            transform.position = other.transform.GetChild(0).transform.position;
            playerScale.ResetPlayerScale();
        }
    }

    private void OnTriggerStay2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Level Zone"))
        {
            GameManager.Instance.currentLevel = collider.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Level Zone"))
        {
            GameManager.Instance.currentLevel = null;
        }
    }

    public void ResetLevelPosition(Vector3 pos)
    {
        transform.position = pos;
    }
}
