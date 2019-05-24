using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using TMPro;

public class LobbyCanvas : MonoBehaviourPunCallbacks
{
    public TextMeshProUGUI lobbyText;
    public TextMeshProUGUI createRoomText;
    public TextMeshProUGUI roomNameText;
    public TextMeshProUGUI returnText;
    public Chat chatController;

    [SerializeField]
    private RoomLayoutGroup roomLayoutGroup;
    public RoomLayoutGroup RoomLayoutGroup
    {
        get { return roomLayoutGroup; }
    }

    [SerializeField]
    private Button createRoomButton;
    public Button CreateRoomButton
    {
        get { return createRoomButton; }
    }

    private void Start()
    {
        roomNameText.text = LocalizationManager.GetTextByKey("ROOM_NAME") + "...";
    }

    public void Update()
    {
        lobbyText.text = LocalizationManager.GetTextByKey("LOBBY");
        createRoomText.text = LocalizationManager.GetTextByKey("CREATE_ROOM");

        returnText.text = LocalizationManager.GetTextByKey("RETURN_TO_MAIN_MENU");
    }

    public void OnClickJoinRoom(string roomName)
    {
        if (PhotonNetwork.JoinRoom(roomName))
        {
            chatController.JoinChat(roomName);
        }
        else
        {
            print("Join room failed.");
        }
    }

    

}
