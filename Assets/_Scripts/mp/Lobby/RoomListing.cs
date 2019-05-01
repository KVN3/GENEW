using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomListing : MonoBehaviour
{
    [SerializeField]
    private Text _roomNameText;
    public Text RoomNameText
    {
        get { return _roomNameText; }
    }

    [SerializeField]
    private Text _playerCountText;
    public Text PlayerCountText
    {
        get { return _playerCountText; }
    }

    public string RoomName { get; private set; }
    public bool Updated { get; set; }

    private void Start()
    {
        GameObject lobbyCanvasObject = MainCanvasManager.instance.LobbyCanvas.gameObject;

        if (lobbyCanvasObject == null)
            return;

        LobbyCanvas lobbyCanvas = lobbyCanvasObject.GetComponent<LobbyCanvas>();

        Button button = GetComponent<Button>();
        button.onClick.AddListener(() => lobbyCanvas.OnClickJoinRoom(RoomNameText.text));
    }

    private void OnDestroy()
    {
        Button button = GetComponent<Button>();
        button.onClick.RemoveAllListeners();
    }

    public void SetRoomNameText(string text)
    {
        RoomName = text;
        RoomNameText.text = RoomName;
    }

    public void SetPlayerCountText(int playerCount, int maxPlayerCount)
    {
        string text = playerCount + "/" + maxPlayerCount;
        PlayerCountText.text = text;
    }
}
