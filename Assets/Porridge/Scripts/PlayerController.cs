using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private float horizontal;
    public float speed = 8f;
    public float jumpingPower = 16f;
    private bool isFacingRight = true;
    public bool hasDoubleJump = false;
    private bool inAir = false;
    public bool isFlippingGravity = false;
    public bool gameHasEnded = false;
    public TextMeshProUGUI skillLastTimeText;
    public TextMeshProUGUI skillCDText;

    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    public Animator animator;
    public Weapon weapon;
    public bool isHit = false;
    public bool isInvinsible = false;
    public float health = 100f;

    public AudioManager audioManager;

    void Update()
    {

        if (gameHasEnded)
        {
            return;
        }

        horizontal = Input.GetAxisRaw("Horizontal");

        if (Input.GetButtonDown("Jump") && IsGrounded())
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpingPower);

            audioManager.PlayJumpSound();
        }

        if (Input.GetButtonUp("Jump") && rb.velocity.y > 0f)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
        }

        // double jump
        if (Input.GetButtonDown("Jump") && !IsGrounded() && !hasDoubleJump)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpingPower);
            hasDoubleJump = true;

            audioManager.PlayJumpSound();
        }

        if (!IsGrounded())
        {

            inAir = true;
        }
        else
        {
            if (inAir)
            {
                Debug.Log("in air, landing play");
                audioManager.PlayLandingSound();
            }
            inAir = false;
        }

        if (IsGrounded())
        {
            hasDoubleJump = false;
            if (animator.GetBool("isJumping"))
            {
                animator.SetBool("isJumping", false);
            }
        }
        else if (!animator.GetBool("isJumping"))
        {
            animator.SetBool("isJumping", true);
        }

        Flip();


        // if press E, flip the gravity for 5 seconds
        if (Input.GetKeyDown(KeyCode.E) && !isFlippingGravity)
        {
            Physics2D.gravity = new Vector2(Physics2D.gravity.x, Physics2D.gravity.y * -1);
            StartCoroutine(FlipGravity());
            isFlippingGravity = true;
            transform.Rotate(0f, 0f, 180f);
        }

        // if press mouse right, melee attack
        if (weapon.isMeleeing)
        {
            // animator.SetTrigger("isMelee");
            animator.Play("PlayerMelee");
        }
        else
        {
            // animator.ResetTrigger("isMelee");
        }

        if (weapon.isShooting)
        {
            Debug.Log("isShooting");
            animator.SetBool("isShooting", true);
        }
        else
        {
            // CHECK WEAPON.CS to see the animation gap functionality
            animator.SetBool("isShooting", false);
        }

        if (isHit && !isInvinsible)
        {
            Debug.Log("player is hit!!!");
            // knock back by a small force 
            rb.velocity = new Vector2(-5f, 5f);
            // flash red
            GetComponent<SpriteRenderer>().color = Color.red;
            health -= 10f;

            // enter invincible state for 0.5 seconds
            StartCoroutine(ResetColor());
        }
    }

    IEnumerator ResetColor()
    {
        isInvinsible = true;
        yield return new WaitForSeconds(0.5f);
        GetComponent<SpriteRenderer>().color = Color.white;
        isHit = false;
        isInvinsible = false;
    }

    private void FixedUpdate()
    {
        if (gameHasEnded)
        {
            return;
        }
        rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);
        if (horizontal != 0f)
        {
            animator.SetBool("isMoving", true);
        }
        else
        {
            animator.SetBool("isMoving", false);
        }
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }

    private void Flip()
    {
        if (isFacingRight && horizontal < 0f || !isFacingRight && horizontal > 0f)
        {
            isFacingRight = !isFacingRight;

            transform.Rotate(0f, 180f, 0f);
        }
    }

    IEnumerator FlipGravity()
    {
        // flip gravity for 3 seconds; update skillLastTimeText to be the remaining time
        float timeLeft = 3;
        while (timeLeft > 0)
        {
            skillLastTimeText.text = "Skill Last Time: " + timeLeft.ToString("F1") + "s";
            timeLeft -= Time.deltaTime;
            yield return null;
        }

        // reset gravity
        Physics2D.gravity = new Vector2(Physics2D.gravity.x, Physics2D.gravity.y * -1);
        transform.Rotate(0f, 0f, 180f);
        StartCoroutine(FlipGravityCD());

    }

    IEnumerator FlipGravityCD()
    {
        // flip gravity CD for 5 seconds; update skillCDText to be the remaining time
        float timeLeft = 5;
        while (timeLeft > 0)
        {
            skillCDText.text = "Skill CD: " + timeLeft.ToString("F1") + "s";
            timeLeft -= Time.deltaTime;
            yield return null;
        }

        // reset CD
        isFlippingGravity = false;
        skillCDText.text = "Skill CD: Ready";
    }

    public void playShootSound()
    {
        audioManager.PlayShootSound();
    }
}