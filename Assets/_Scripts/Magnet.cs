using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Magnet : MonoBehaviour
{
    public GameObject metalObj;
    public float magneticForce;

    private Rigidbody2D rb;
    private Collider2D col;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {
        Rigidbody2D metalRb = metalObj.GetComponent<Rigidbody2D>();
        Collider2D metalCollider = metalObj.GetComponent<Collider2D>();

        float magnetArea = CalcArea(gameObject);
        float metalArea = CalcArea(metalObj.gameObject);

        Vector2 metalDir = (V3ToV2(transform.position) - metalCollider.ClosestPoint(transform.position)).normalized;
        Vector2 magnetDir = (V3ToV2(metalObj.transform.position) - col.ClosestPoint(metalObj.transform.position)).normalized;
        // Force is stronger if magnet is larger
        if (metalArea != 0)
        {
            float forceMultiplier = magneticForce * (magnetArea / metalArea);

            metalRb.AddForce(metalDir * forceMultiplier);
            // rb.AddForce(magnetDir * forceMultiplier);
        }
    }

    private float CalcArea(GameObject obj)
    {
        Collider2D collider = obj.GetComponent<Collider2D>();
        return collider.bounds.size.x * collider.bounds.size.y;
    }

    private Vector2 V3ToV2(Vector3 v)
    {
        return new Vector2(v.x, v.y);
    }

    private Vector3 V2ToV3(Vector2 v)
    {
        return new Vector3(v.y, v.x, 0);
    }
}
