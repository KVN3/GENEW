using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MoverManager : EnemyManager
{
    protected override void SpawnEnemy()
    {
        base.SpawnEnemy();

        //RandomMover moverClass = enemyClasses[Random.Range(0, enemyClasses.Length)] as RandomMover;
        //moverClass.xMax = sp.xMax;
        //moverClass.xMin = sp.xMin;
        //moverClass.zMax = sp.zMax;
        //moverClass.zMin = sp.zMin;
        //moverClass.maxSpeed = sp.maxSpeed;
        //RandomMover mover = Instantiate(moverClass, sp.transform.position, Quaternion.identity);



        //mover.shooterModule.SetTargets(players);
        //mover.SetManager(this);
        //enemies.Add(mover);

        GetComponent<PhotonView>().RPC("RPC_SpawnMover", RpcTarget.AllViaServer);
    }

    [PunRPC]
    public void RPC_SpawnMover(PhotonMessageInfo info)
    {
        string moverPath = SharedResources.GetPath("MoverEnemy");

        RandomMover mover = PhotonNetwork.Instantiate(moverPath, sp.transform.position, sp.transform.rotation).GetComponent<RandomMover>();

        // Bounds
        mover.xMax = sp.xMax;
        mover.xMin = sp.xMin;
        mover.zMax = sp.zMax;
        mover.zMin = sp.zMin;
        mover.maxSpeed = sp.maxSpeed;

        // Shooter module
        Shooter shooterModule = mover.GetComponent<Shooter>();
        shooterModule.SetTargets(players);
        mover.shooterModule = shooterModule;

        mover.SetManager(this);
        enemies.Add(mover);
    }
}
