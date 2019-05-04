using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JammerProjectile : Collectable
{
    public ElectricExplosion explosionClass;
    public float projectileSpeed;
    public float maxDrag;

    private ElectricExplosion explosion;
    public Ship owner;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(WaitAndDestroy());
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Rigidbody rb = GetComponent<Rigidbody>();

        Vector3 forward = transform.forward * Time.deltaTime * projectileSpeed;
        rb.AddForce(forward);

        if (rb.drag < maxDrag)
            rb.drag = rb.drag + 0.05f;
    }

    void OnCollisionEnter(Collision other)
    {
        // Projectile does not explode when touching owner
        if (other.gameObject.GetComponent<Ship>() == owner)
        {
            Debug.Log("Bullet hit owner...");
        }
        else
        {
            explosion = Instantiate(explosionClass, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }

    // Explode and destroy projectile
    IEnumerator Explode(Vector3 position)
    {
        explosion = Instantiate(explosionClass, position, Quaternion.identity);
        Destroy(gameObject);
        yield return new WaitForSeconds(10);
    }

    IEnumerator WaitAndDestroy()
    {
        yield return new WaitForSeconds(10);
        Destroy(gameObject);
    }


}
