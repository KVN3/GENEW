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

        string roomName = GenerateRoomName();

        // Creates room
        if (PhotonNetwork.CreateRoom(roomName, roomOptions, TypedLobby.Default))
        {
            CurrentRoomCanvas.instance.RoomName = roomName;

            if (tutorial)
            {
                GameConfiguration.tutorial = true;
                print("Create tutorial room successfully sent.");
            }
            else
            {
                GameConfiguration.tutorial = false;
                // Join chat channel of the room
                Chat.instance.JoinChat(roomName);
                Chat.instance.roomName = roomName;
                print("Create room successfully sent.");
            }
        }
        else
        {
            print("Create room failed to send.");
        }
    }

    private string GenerateRoomName()
    {
        bool validName = false;
        string roomName = RoomName.text;

        if (RoomName.text.Equals("Room name..."))
        {
            roomName = "ROOM";
        }

        
        // While name is not valid
        while (!validName)
        {
            roomName = roomName + "#" + Random.Range(10000, 99999).ToString();

            // If room doesn't exist, validname
            if (RoomLayoutGroup.Instance.RoomExists(roomName))
            {
                
            }
            else
            {
                validName = true;
            }
        }        

        return roomName;
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
