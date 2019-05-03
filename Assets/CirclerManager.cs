using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CirclerManager : MonoBehaviour
{
    [SerializeField]
    private Transform orbitHallOfBall;
    [SerializeField]
    private int amountOfBalls;

    private PhotonView photonView;

    public void Awake()
    {
        photonView = GetComponent<PhotonView>();
        photonView.ViewID = 796;
    }

    public void SpawnCirclers()
    {
        photonView.RPC("RPC_SpawnCirclers", RpcTarget.AllViaServer);
    }

    [PunRPC]
    public void RPC_SpawnCirclers(PhotonMessageInfo info)
    {
        string circlerPath = SharedResources.GetPath("CirclingEnergyBall");

        // Circling chain
        for (int i = 0; i < amountOfBalls; i++)
        {
            EnergyBall circler = PhotonNetwork.Instantiate(circlerPath, transform.position * Random.Range(0f, 10f), transform.rotation).GetComponent<EnergyBall>();

            // Circling settings
            RotateAroundObject circlerSettings = circler.GetComponent<RotateAroundObject>();
            circlerSettings.target = orbitHallOfBall;
            circlerSettings.orbitDistance = i * 20;
            circlerSettings.orbitDegreesPerSec = -120;
            circlerSettings.heightY = 10;
        }


    }
}
