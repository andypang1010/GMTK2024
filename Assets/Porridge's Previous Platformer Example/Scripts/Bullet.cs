using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 20f;
    public GameObject ImpactEffect;
    public LayerMask whatIsSolid;
    // Start is called before the first frame update
    void Start()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.velocity = transform.right * speed;

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter2D(Collider2D hitInfo)
    {
        if (hitInfo.tag == "Enemy")
        {
            hitInfo.GetComponent<AIHoriBrain>().isHit = true;
            Destroy(gameObject);
            Instantiate(ImpactEffect, transform.position, transform.rotation);
            return;
        }

        // compare layer with whatIsSolid
        if ((whatIsSolid.value & 1 << hitInfo.gameObject.layer) == 1 << hitInfo.gameObject.layer)
        {
            Destroy(gameObject);
            Instantiate(ImpactEffect, transform.position, transform.rotation);
        }
        // Instantiate(ImpactEffect, transform.position, transform.rotation);
    }
}
