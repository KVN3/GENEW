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

    void Awake()
    {
        photonView = GetComponent<PhotonView>();


    }

    void Start()
    {
        // Find the sound manager
        sm = transform.Find("EnemySoundManager(Clone)").gameObject.GetComponent<EnemySoundManager>();
        if (sm == null)
            sm = transform.Find("Enemy Sound Manager(Clone)").gameObject.GetComponent<EnemySoundManager>();

    }

    // Shoot a projectile at a Vector3 target
    public void Fire(Vector3 target)
    {
        if (isCloseEnough)
        {
            //string projectilePath = SharedResources.GetPath("EnergyBallProjectile");
            //EnergyBallProjectile projectile = PhotonNetwork.Instantiate(projectilePath, this.transform.position, this.transform.rotation).GetComponent<EnergyBallProjectile>();

            //projectile.target = target;

            //if (sm == null)
            //    print("Sound Manager null");
            //else
            //    sm.PlaySound(SoundType.SHOOTING);

            photonView.RPC("RPC_ShootAtTarget", RpcTarget.AllViaServer, target);
        }
    }

    [PunRPC]
    public void RPC_ShootAtTarget(Vector3 target, PhotonMessageInfo info)
    {
        float lag = (float)(PhotonNetwork.Time - info.SentServerTime);

        EnergyBallProjectile projectileClass = energyBallProjectileClasses[Random.Range(0, energyBallProjectileClasses.Length)];
        EnergyBallProjectile projectile = Instantiate(projectileClass, this.transform.position, this.transform.rotation);

        //EnergyBallProjectile projectile = PhotonNetwork.Instantiate(SharedResources.GetPath("EnergyBallProjectile"), this.transform.position, this.transform.rotation).GetComponent<EnergyBallProjectile>();

        projectile.target = target;

        if (sm == null)
            print("Sound Manager null");
        else
            sm.PlaySound(SoundType.SHOOTING);
    }

    // Locates the closest enemy ship to gun for
    public Vector3 GetClosestTarget()
    {
        bool withinRange = false;

        Vector3 closestTarget = Vector3.zero;
        PlayerShip closestShip = new PlayerShip();

        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = transform.position;

        // Go through each ship in the list of targets
        foreach (PlayerShip target in targets)
        {
            Vector3 potentialTargetPosition = target.transform.position;

            Vector3 directionToTarget = potentialTargetPosition - currentPosition;

            // distance to target
            float dSqrToTarget = directionToTarget.sqrMagnitude;

            // If closer than last found ship
            if (dSqrToTarget < closestDistanceSqr)
            {
                // This is now the closest ship
                closestDistanceSqr = dSqrToTarget;

                // If within detection range
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