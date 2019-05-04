using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JammerMine : Collectable
{
    public ElectricExplosion explosionClass;
    public Ship owner;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(WaitAndDestroy());
    }

    void OnTriggerEnter(Collider other)
    {
        bool shouldExplode = false;

        if (other.gameObject.CompareTag("EnergyBall"))
        {
            shouldExplode = true;
        }
        
        if (other.gameObject.CompareTag("Ship"))
        {
            Ship ship = other.gameObject.GetComponent<Ship>();

            if (ship.Equals(owner))
            {
                print("Mine triggered by owner");
            }
            else
            {
                print("Mine triggered by opp ship");
                shouldExplode = true;
            }
        }

        if (shouldExplode)
        {
            Instantiate(explosionClass, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }

    IEnumerator WaitAndDestroy()
    {
        yield return new WaitForSeconds(120);
        Destroy(gameObject);
    }


}
