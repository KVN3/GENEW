using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Lifetime;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    public Explosion explosionClass;
    private Explosion explosion;
    public AsteroidStormManager manager;

    public int lifeTimeInSeconds;
    public float floatSpeed = 10f;

    public virtual void Start()
    {
        if (lifeTimeInSeconds != 0)
            StartCoroutine(WaitAndDestroy());
    }

    IEnumerator Explode(Vector3 position)
    {
        explosion = Instantiate(explosionClass, position, Quaternion.identity);
        yield return new WaitForSeconds(1);
        DestroyThisAsteroid();
    }

    IEnumerator WaitAndDestroy()
    {
        yield return new WaitForSeconds(8);
        DestroyThisAsteroid();
    }

    void OnCollisionEnter(Collision other)
    {
        StartCoroutine(Explode(transform.position));
        if (other.gameObject.CompareTag("Ship"))
        {
            Rigidbody rb = this.GetComponent<Rigidbody>();
            Rigidbody shipRb = other.gameObject.GetComponent<Rigidbody>();

            StartCoroutine(SpawnBackShip(other));

            shipRb.velocity = Vector3.left * 100;
        }
    }

    public IEnumerator SpawnBackShip(Collision other)
    {
        Vector3 initialPos = other.gameObject.transform.position;
        yield return new WaitForSeconds(5);
        other.gameObject.transform.position = initialPos;
    }

    private void DestroyThisAsteroid()
    {
        Destroy(gameObject);
        manager.RemoveAsteroidFromObjectList(this);
    }
}
