using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    private Rigidbody2D rb;

    [Header("Movement")]
    [SerializeField] private float moveSpeed;
    private float horizontal;

    [Header("Jump")]
    [SerializeField] private float jumpForce;
    [SerializeField] private float coyoteTime;
    [SerializeField] private float jumpBuffer;
    private float coyoteTimeCounter, jumpBufferCounter;

    void Start() {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        horizontal = Input.GetAxisRaw("Horizontal");


        // Coyote Time Check
        if (IsGrounded()) {
            coyoteTimeCounter = coyoteTime;
        }
        else {
            coyoteTimeCounter -= Time.deltaTime;
        }

        // Jump Buffer Check
        if (Input.GetKeyDown(KeyCode.Space)) {
            jumpBufferCounter = jumpBuffer;
        }
        else {
            jumpBufferCounter -= Time.deltaTime;
        }

        if (coyoteTimeCounter > 0f && jumpBufferCounter > 0f) {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }

        // // Hold jump to jump higher
        // if (Input.GetKeyUp(KeyCode.Space) && rb.velocity.y > 0f) {
        //     rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);

        //     coyoteTimeCounter = 0f;
        // }
    }

    void FixedUpdate() {
        if (IsGrounded()) {
            rb.velocity = new Vector2(horizontal * moveSpeed, rb.velocity.y);
        }

        // Damping in air
        else {
            rb.velocity = new Vector2(horizontal * moveSpeed * 0.7f, rb.velocity.y);
        }
    }

    bool IsGrounded() {
        return Physics2D.OverlapCircle(groundCheck.position, 0.05f, groundLayer);
    }
}
