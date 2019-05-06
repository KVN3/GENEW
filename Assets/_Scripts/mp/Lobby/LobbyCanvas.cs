using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class LobbyCanvas : MonoBehaviourPunCallbacks
{
    public Text createRoomText;
    public Text roomNameText;
    public Text returnText;

    [SerializeField]
    private RoomLayoutGroup _roomLayoutGroup;
    private RoomLayoutGroup RoomLayoutGroup
    {
        get { return _roomLayoutGroup; }
    }

    public void Update()
    {
        createRoomText.text = LocalizationManager.GetTextByKey("CREATE_ROOM");
        roomNameText.text = LocalizationManager.GetTextByKey("ROOM_NAME") + "...";
        returnText.text = LocalizationManager.GetTextByKey("RETURN_TO_MAIN_MENU");
    }

    public void OnClickJoinRoom(string roomName)
    {
       if(PhotonNetwork.JoinRoom(roomName))
        {

        }
        else
        {
            print("Join room failed.");
        }
    }

}
