using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public GameObject firePoint;
    public GameObject bulletPrefab;
    public bool isShooting = false;
    private bool isCountingDown = false;
    // this is to prevent the shooting variable to switch TOO fast 
    // that the animation is not shown
    public float shootingAnimationGap = 1f;

    public GameObject meleeRange;
    public bool isMeleeing = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            isMeleeing = true;
        }
        if (!Input.GetMouseButton(1))
        {
            isMeleeing = false;
        }
        if (Input.GetButtonDown("Fire1"))
        {
            Shoot();

        }
        // if Fire1 is NOT PRESSED, not get button up, set isShooting to false
        // NOT GET BUTTON UP!
        if (!Input.GetButton("Fire1") && !isCountingDown)
        {
            isShooting = false;
        }
    }

    void Shoot()
    {

        isShooting = true;
        StartCoroutine(ShootingAnimation());
        // destroy bullet after 2 seconds
    }

    IEnumerator ShootingAnimation()
    {
        isCountingDown = true;
        yield return new WaitForSeconds(shootingAnimationGap);
        isCountingDown = false;
    }

    void InstantiateBullet()
    {
        GameObject bullet = Instantiate(bulletPrefab, firePoint.transform.position, firePoint.transform.rotation);
        Destroy(bullet, 2f);
    }

    public void MeleeAttack()
    {
        // turn meleerange into red color by setting its opacity
        meleeRange.GetComponent<SpriteRenderer>().color = new Color(1, 0, 0, 0.5f);
        // turn it back to white after 0.5 seconds
        StartCoroutine(ResetMeleeColor());
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(meleeRange.transform.position, 0.5f);
        foreach (Collider2D enemy in hitEnemies)
        {
            if (enemy.CompareTag("Enemy"))
            {
                enemy.GetComponent<AIHoriBrain>().isHit = true;
            }
        }
    }

    IEnumerator ResetMeleeColor()
    {
        yield return new WaitForSeconds(0.5f);
        // set it back to transparent
        meleeRange.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
    }
}
