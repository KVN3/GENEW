using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShooterController : MonoBehaviour
{
    public Shooter shooter;

    void Start()
    {
        StartCoroutine(PerformFiring());
    }

    IEnumerator PerformFiring()
    {
        while (true)
        {
            Vector3 target = shooter.GetClosestTarget();
            shooter.Fire(target);
            yield return new WaitForSeconds(Random.Range(1f, 3f));
        }
    }
}
