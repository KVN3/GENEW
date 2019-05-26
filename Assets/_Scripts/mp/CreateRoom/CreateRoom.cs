using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CreateRoom : MonoBehaviourPunCallbacks, IMatchmakingCallbacks
{
    public GameObject currentRoomHeader;

    [SerializeField]
    private TextMeshProUGUI _roomName;
    private TextMeshProUGUI RoomName
    {
        get { return _roomName; }
    }

    // Create Room
    public void OnClick_CreateRoom(bool tutorial)
    {
        // Set the room options
        RoomOptions roomOptions = new RoomOptions() { IsVisible = true, IsOpen = true, MaxPlayers = 3, PublishUserId = true }; // PublisherId is for making friends

        // Creates room
        if (PhotonNetwork.CreateRoom(RoomName.text, roomOptions, TypedLobby.Default))
        {
            if (tutorial)
            {
                GameConfiguration.tutorial = true;
                print("Create tutorial room successfully sent.");
            }
            else
            {
                GameConfiguration.tutorial = false;
                Chat.instance.JoinChat(RoomName.text);
                print("Create room successfully sent.");
            }

            
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

        if (GameConfiguration.tutorial)
        {
            CurrentRoomCanvas.instance.OnClickStartDelayed(true);
        }

    }
}
