using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShooterController : MonoBehaviour
{
    public Shooter shooter;
    public float minCooldown, maxCooldown;

    void Start()
    {
        //if (PhotonNetwork.IsMasterClient)
        StartCoroutine(PerformFiring());
    }

    IEnumerator PerformFiring()
    {
        while (true)
        {
            Vector3 target = shooter.GetClosestTarget();
            shooter.Fire(target);
            yield return new WaitForSeconds(Random.Range(minCooldown, maxCooldown));
        }
    }
}
