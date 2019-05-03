using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Shooter : MonoBehaviour
{
    public EnergyBallProjectile[] energyBallProjectileClasses;
    public float minDistance = 400000;

    public PlayerShip[] targets;

    private EnemySoundManager sm;
    private bool isCloseEnough = false;

    private PhotonView photonView;
    private Vector3 currentTarget;

    void Awake()
    {
        photonView = GetComponent<PhotonView>();
    }

    // Shoot a projectile at a Vector3 target
    public void Fire(Vector3 target)
    {
        DestroyIfNoParent();

        if (isCloseEnough)
        {
            currentTarget = target;
            photonView.RPC("RPC_ShootAtTarget", RpcTarget.AllViaServer);
        }
    }

    [PunRPC]
    public void RPC_ShootAtTarget(PhotonMessageInfo info)
    {
        float lag = (float)(PhotonNetwork.Time - info.SentServerTime);

        sm.PlaySound(SoundType.SHOOTING);

        //EnergyBallProjectile projectile = energyBallProjectileClasses[Random.Range(0, energyBallProjectileClasses.Length)];
        //Instantiate(projectile, this.transform.position, this.transform.rotation);

        EnergyBallProjectile projectile = PhotonNetwork.Instantiate(SharedResources.GetPath("EnergyBallProjectile"), transform.position, transform.rotation).GetComponent<EnergyBallProjectile>();
        projectile.target = currentTarget;
    }

    // Destroy this object if it has no parent object
    private void DestroyIfNoParent()
    {
        if (transform.parent == null)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }

    // Locates the closest enemy ship to gun for
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