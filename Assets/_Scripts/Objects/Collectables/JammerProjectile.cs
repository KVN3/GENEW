using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct JammerProjectileConfig
{
    [Tooltip("The explosion Particle Effect prefab.")]
    public ElectricExplosion explosionClass;

    [Tooltip("Base speed of the projectile.")]
    public float projectileSpeed;

    [Tooltip("Maximum air drag / resistance value.")]
    public float maxDrag;

    [Tooltip("Max life time in seconds. Object will disappear after this time's passed and no explosion's occurred.")]
    public float lifetimeInSeconds;
}

public class JammerProjectile : Collectable
{
    [SerializeField]
    private JammerProjectileConfig config;

    private ElectricExplosion explosion;
    public Ship owner;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(this, config.lifetimeInSeconds);
    }

    
    void FixedUpdate()
    {
        Rigidbody rb = GetComponent<Rigidbody>();

        ApplyForwardForce(rb);
        UpdateDrag(rb);
    }

    // Updates the air resistance drag value
    private void UpdateDrag(Rigidbody rb)
    {
        if (rb.drag < config.maxDrag)
            rb.drag += 0.05f;
    }

    // Adds forward force to the projectile
    private void ApplyForwardForce(Rigidbody rb)
    {
        Vector3 forward = transform.forward * Time.deltaTime * config.projectileSpeed;
        rb.AddForce(forward);
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
            explosion = Instantiate(config.explosionClass, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }

    // Explode and destroy projectile
    IEnumerator Explode(Vector3 position)
    {
        explosion = Instantiate(config.explosionClass, position, Quaternion.identity);
        Destroy(gameObject);
        yield return new WaitForSeconds(10);
    }


}
