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

        GetComponent<PhotonView>().RPC("RPC_SpawnMover", RpcTarget.AllViaServer);
    }

    [PunRPC]
    public void RPC_SpawnMover(PhotonMessageInfo info)
    {
        string moverPath = SharedResources.GetPath("MoverEnemy");

        Vector3 lsp = new Vector3(sp.transform.position.x + Random.Range(-20f, 20f), sp.transform.position.y, sp.transform.position.z + Random.Range(-20f, 20f));

        RandomMover mover = PhotonNetwork.Instantiate(moverPath, lsp, sp.transform.rotation).GetComponent<RandomMover>();

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
