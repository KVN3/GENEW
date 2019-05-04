using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ChaserManager : EnemyManager
{
    protected override void SpawnEnemy()
    {
        base.SpawnEnemy();

        GetComponent<PhotonView>().RPC("RPC_SpawnChaser", RpcTarget.AllViaServer);
    }

    [PunRPC]
    public void RPC_SpawnChaser(PhotonMessageInfo info)
    {
        string chaserPath = Path.Combine("Prefabs", "Enemies", "Chasers", "ChaserEnemy");

        Chaser chaser = PhotonNetwork.Instantiate(chaserPath, sp.transform.position, sp.transform.rotation).GetComponent<Chaser>();
        chaser.SetTargets(players);
        chaser.SetManager(this);
        enemies.Add(chaser);
    }
}
