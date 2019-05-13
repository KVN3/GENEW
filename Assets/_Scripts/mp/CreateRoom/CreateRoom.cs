using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreateRoom : MonoBehaviourPunCallbacks, IMatchmakingCallbacks
{
    [SerializeField]
    private Text _roomName;
    private Text RoomName
    {
        get { return _roomName; }
    }

    // Create Room
    public void OnClick_CreateRoom()
    {
        // Set the room options
        RoomOptions roomOptions = new RoomOptions() { IsVisible = true, IsOpen = true, MaxPlayers = 3, PublishUserId = true }; // PublisherId is for making friends

        // Creates room
        if (PhotonNetwork.CreateRoom(RoomName.text, roomOptions, TypedLobby.Default))
        {
            Chat.instance.JoinChat(RoomName.text);
            print("Create room successfully sent.");
        }
        else
        {
            print("Create room failed to send.");
        }
    }

    public void OnPhotonCreateRoomFailed(object[] codeAndMessage)
    {
        print("Create room failed: " + codeAndMessage[1]);
    }

    public override void OnCreatedRoom()
    {
        print("Room created successfully");
    }
}
