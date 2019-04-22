using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooter : MonoBehaviour
{
    public EnergyBallProjectile[] energyBallProjectileClasses;
    public float minDistance = 400000;

    public PlayerShip[] targets;

    private EnemySoundManager sm;
    private bool isCloseEnough = false;

    public void Fire(Vector3 target)
    {
        DestroyIfNoParent();
        Debug.Log("Fire called.");

        if (isCloseEnough)
        {
            Debug.Log("Close enough and firing.");
            sm.PlaySound(SoundType.SHOOTING);
            EnergyBallProjectile projectile = energyBallProjectileClasses[Random.Range(0, energyBallProjectileClasses.Length)];
            projectile.target = target;

            Instantiate(projectile, this.transform.position, this.transform.rotation);
        }
    }

    private void DestroyIfNoParent()
    {
        if (transform.parent == null)
        {
            Destroy(gameObject);
        }
    }

    public Vector3 GetClosestTarget()
    {
        bool withinRange = false;

        Vector3 closestTarget = Vector3.zero;
        PlayerShip closestShip = new PlayerShip();

        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = transform.position;

        foreach (PlayerShip target in targets)
        {
            Vector3 potentialTargetPosition = target.transform.position;

            Vector3 directionToTarget = potentialTargetPosition - currentPosition;

            float dSqrToTarget = directionToTarget.sqrMagnitude;

            if (dSqrToTarget < closestDistanceSqr)
            {
                closestDistanceSqr = dSqrToTarget;

                if (closestDistanceSqr < minDistance)
                {
                    withinRange = true;
                    closestTarget = potentialTargetPosition;
                    closestShip = target;
                }
            }
        }

        if (withinRange)
        {
            this.isCloseEnough = true;
            closestShip.Alert();
        }
        else
            this.isCloseEnough = false;

        Debug.Log("Returning target for shooter... close enough = " + withinRange);

        return closestTarget;
    }

    public void SetTargets(PlayerShip[] targets)
    {
        this.targets = targets;
    }

    public void SetSoundManager(EnemySoundManager sm)
    {
        this.sm = sm;
    }
}