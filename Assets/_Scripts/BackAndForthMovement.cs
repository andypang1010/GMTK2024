using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackAndForthMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform forwardCheck;
    private Rigidbody2D rb;

    [Header("Movement")]
    [SerializeField] private float moveSpeed;
    private int horizontal;

    // Start is called before the first frame update
    void Start()
    {
        horizontal = 1;
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (IsHittingWall())
        {
            print("Is Hitting wall");
            horizontal *= -1;
        }

        Flip();
    }

    public void Flip()
    {
        if (Mathf.Sign(horizontal) == Mathf.Sign(transform.localScale.x)) return;

        else
        {
            transform.localScale = Vector3.Scale(transform.localScale, new Vector3(-1, 1, 1));
        }
    }

    private void FixedUpdate()
    {
        rb.velocity = new Vector2(horizontal * moveSpeed, rb.velocity.y);
    }

    private bool IsHittingWall()
    {
        Collider2D[] colliders = Physics2D.OverlapBoxAll(forwardCheck.position, GetForwarCheckBoxDimension(), 0f);
        
        foreach (Collider2D collider in colliders) {
            if (collider.gameObject == gameObject) continue;
            return true;
        }

        return false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawCube(forwardCheck.position, GetForwarCheckBoxDimension());
    }

    private Vector2 GetForwarCheckBoxDimension()
    {
        return new Vector2(0.05f, 0.75f * Mathf.Abs(transform.localScale.y));
    }
}
