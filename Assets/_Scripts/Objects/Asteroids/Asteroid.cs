using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Lifetime;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    public SmokeScreen smokeScreenClass;
    public Explosion explosionClass;
    public AsteroidStormManager manager;

    public int lifeTimeInSeconds;
    public float floatSpeed = 10f;

    public virtual void Start()
    {
        if (lifeTimeInSeconds != 0)
            StartCoroutine(WaitAndDestroy());
    }

    //IEnumerator Explode(Vector3 position)
    //{
    //    smokeScreen = Instantiate(smokeScreenClass, position, Quaternion.identity);
    //    yield return new WaitForSeconds(1);
    //    DestroyThisAsteroid();
    //}

    IEnumerator WaitAndDestroy()
    {
        yield return new WaitForSeconds(8);
        DestroyThisAsteroid();
    }

    void OnCollisionEnter(Collision other)
    {
        Instantiate(explosionClass, this.transform.position, Quaternion.identity);
        Instantiate(smokeScreenClass, this.transform.position, Quaternion.identity);
        DestroyThisAsteroid();

        if (other.gameObject.CompareTag("Ship"))
        {
            Ship ship = other.gameObject.GetComponent<Ship>();
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
