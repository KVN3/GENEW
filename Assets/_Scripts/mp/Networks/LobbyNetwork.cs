using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyNetwork : MonoBehaviourPunCallbacks
{

    private void Start()
    {

        Connect();
        
    }

    public static void Connect()
    {
        if (PhotonNetwork.IsConnected)
            return;

        print("Connecting to server...");

        PhotonNetwork.GameVersion = "0.0.0";
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        print("Connected to master.");

        // Automatically join host's scene
        PhotonNetwork.AutomaticallySyncScene = false;

        PhotonNetwork.NickName = PlayerNetwork.Instance.PlayerName;
        PhotonNetwork.JoinLobby(TypedLobby.Default);
    }

    public override void OnJoinedLobby()
    {
        print("Joined lobby.");

        if (!PhotonNetwork.InRoom)
        {
            MainCanvasManager.instance.LobbyCanvas.transform.SetAsLastSibling();
            MainCanvasManager.instance.CurrentRoomCanvas.transform.SetAsFirstSibling();
        }

    }



}
