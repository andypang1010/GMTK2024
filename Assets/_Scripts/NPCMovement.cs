using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform forwardCheck;
    [SerializeField] private LayerMask teaLayer;
    [SerializeField] private List<Collider2D> ignoreColliders;
    private Rigidbody2D rb;
    public Animator npcAnim;

    [Header("Movement")]
    [SerializeField] private float moveSpeed;
    private int horizontal;
    private Vector3 spawnPosition;
    private float respawnTimer = 0;

    // Start is called before the first frame update
    void Start()
    {
        horizontal = 1;
        rb = GetComponent<Rigidbody2D>();
        spawnPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        respawnTimer -= Time.deltaTime;
        if (respawnTimer <= 0)
        {
            respawnTimer = 0;
        }

        if (IsHittingWall())
        {
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
        if (respawnTimer > 0) return;
        rb.velocity = new Vector2(horizontal * moveSpeed * GetWorldSize().x, rb.velocity.y);
    }

    private bool IsHittingWall()
    {
        Collider2D[] colliders = Physics2D.OverlapBoxAll(forwardCheck.position, GetForwarCheckBoxDimension(), 0f, ~LayerMask.GetMask("UI"));

        foreach (Collider2D collider in colliders)
        {
            if (collider.gameObject == gameObject || ignoreColliders.Contains(collider)) continue;
            Debug.Log(gameObject.name + " colliding with: " + collider.gameObject.name);
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
        Vector2 colliderSize = GetComponent<Collider2D>().bounds.size;
        return new Vector2(0.05f * colliderSize.x, 0.8f * colliderSize.y);
    }

    private Vector2 GetWorldSize()
    {
        return GetComponent<Collider2D>().bounds.size;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {

        // If the collider is of a layer in the Tea Layer
        if ((teaLayer.value & 1 << other.gameObject.layer) > 0)
        {
            ResetPosition();
        }
    }

    public void ResetPosition()
    {
        transform.position = spawnPosition;

        npcAnim.SetTrigger("Respawn");
        respawnTimer = 1;
    }
}
