using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class ElectricExplosion : MonoBehaviour
{
    public int shutDownDuration = 4;
    public int lifeTimeDuration = 5;

    void Start()
    {


        //audioSource.clip = explosionSoundClip;
        //audioSource.Play();

        StartCoroutine(WaitAndDestroy());
    }

    IEnumerator WaitAndDestroy()
    {
        yield return new WaitForSeconds(lifeTimeDuration);
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Ship"))
        {
            PlayerShip playerShip = other.GetComponent<PlayerShip>();
            Rigidbody rb = other.GetComponent<Rigidbody>();

            playerShip.GetStunned(shutDownDuration, "Player weapon");
        }
        else if (other.gameObject.CompareTag("EnergyBall"))
        {
            EnergyBall energyBall = other.GetComponent<EnergyBall>();
            energyBall.Die();
        }
    }
}
