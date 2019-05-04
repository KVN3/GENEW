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
        if (!PhotonNetwork.IsMasterClient)
            return;

        if (other.gameObject.CompareTag("Ship") || other.gameObject.CompareTag("EnergyBall"))
        {
            if (!other.gameObject.GetComponent<Ship>() == owner)
            {
                Instantiate(explosionClass, transform.position, Quaternion.identity);
                Destroy(gameObject);
            }
        }
    }

    IEnumerator WaitAndDestroy()
    {
        yield return new WaitForSeconds(120);
        Destroy(gameObject);
    }


}
