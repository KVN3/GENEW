using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spike : MonoBehaviour
{
    public void OnTriggerEnter(Collider other)
    {
        MeshExplosion meshExplosion = GetComponent<MeshExplosion>();

        if (other.gameObject.CompareTag("Ship"))
        {
            // Less speed due to impact
            Rigidbody rb = other.gameObject.GetComponent<Rigidbody>();
            rb.velocity = new Vector3(rb.velocity.x / 1.5f, rb.velocity.y, rb.velocity.z / 1.5f);

            meshExplosion.ExplodeMesh();
        }
        else if (other.gameObject.CompareTag("Projectile"))
        {
            meshExplosion.ExplodeMesh();
        }
    }
}
