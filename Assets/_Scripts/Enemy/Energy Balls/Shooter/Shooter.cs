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
        if (isCloseEnough)
        {
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
        bool isCloseEnough = false;

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
                    isCloseEnough = true;
                    closestTarget = potentialTargetPosition;
                    closestShip = target;
                }
            }
        }

        if (isCloseEnough)
        {
            closestShip.Alert();
        }
        else
            this.isCloseEnough = false;

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