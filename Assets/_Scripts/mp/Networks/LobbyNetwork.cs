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

        StartCoroutine(C_ManageButtonAccess());
    }

    public static void Connect()
    {
        if (PhotonNetwork.IsConnected)
            return;

        print("Connecting to server...");

        PhotonNetwork.GameVersion = "0.0.0";
        Account account = Registration.GetCurrentAccount();
        // Sets UserId to account username instead of a randomly generated unique cod
        PhotonNetwork.AuthValues = new AuthenticationValues(PlayerNetwork.Instance.PlayerName);

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
    }

    private IEnumerator C_ManageButtonAccess()
    {
        while (true)
        {
            if (PhotonNetwork.IsConnectedAndReady && PhotonNetwork.InLobby)
            {
                MainCanvasManager.instance.LobbyCanvas.CreateRoomButton.interactable = true;
                MainCanvasManager.instance.MainMenu.tutorialButton.interactable = true;
            }
            else
            {
                MainCanvasManager.instance.LobbyCanvas.CreateRoomButton.interactable = false;
                MainCanvasManager.instance.MainMenu.tutorialButton.interactable = false;
            }

            yield return new WaitForSeconds(.3f);
        }
    }



}
