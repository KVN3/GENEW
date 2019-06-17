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

        //GetComponent<PhotonView>().RPC("RPC_SpawnChaser", RpcTarget.AllViaServer);

        string chaserPath = Path.Combine("Prefabs", "Enemies", "Chasers", "ChaserEnemy");

        Vector3 lsp = new Vector3(sp.transform.position.x + Random.Range(-20f, 20f), sp.transform.position.y, sp.transform.position.z + Random.Range(-20f, 20f));

        Chaser chaser = PhotonNetwork.Instantiate(chaserPath, lsp, sp.transform.rotation).GetComponent<Chaser>();
        chaser.SetTargets(players);
        chaser.SetManager(this);
        enemies.Add(chaser);
    }

    [PunRPC]
    public void RPC_SpawnChaser(PhotonMessageInfo info)
    {
        string chaserPath = Path.Combine("Prefabs", "Enemies", "Chasers", "ChaserEnemy");

        Vector3 lsp = new Vector3(sp.transform.position.x + Random.Range(-20f, 20f), sp.transform.position.y, sp.transform.position.z + Random.Range(-20f, 20f));

        Chaser chaser = PhotonNetwork.Instantiate(chaserPath, lsp, sp.transform.rotation).GetComponent<Chaser>();
        chaser.SetTargets(players);
        chaser.SetManager(this);
        enemies.Add(chaser);
    }
}
