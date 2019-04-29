using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.Events;

public class GameInstance : MonoBehaviourPunCallbacks
{
    public UnityAction OnJoinedLobbyDelegate;
    public UnityAction OnJoinedRoomDelegate;

    public UnityAction<Player> OnPlayerJoinedDelegate;
    public UnityAction<Player> OnPlayerLeftDelegate;

    private UnityAction OnCreatedRoomDelegate;
    private UnityAction<short, string> OnCreateRoomFailedDelegate;

    private byte maxPlayers;
    private bool logging;

    void Awake()
    {
        logging = false;
        DontDestroyOnLoad(this);
        maxPlayers = 3;
    }

    void Start()
    {
    }

    public static void Connect()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        bool Success = PhotonNetwork.ConnectUsingSettings();

        if (!Success)
        {
            if (GameInstance.Instance.logging)
                Debug.Log($"Failed connecting to Photon");
        }
        else
        {
            
        }
    }

    public void CreateRoom(string Name, UnityAction Success, UnityAction<short, string> Failed)
    {
        OnCreatedRoomDelegate = () =>
        {
            OnCreatedRoomDelegate = null;
            OnCreateRoomFailedDelegate = null;

            Success();
        };

        OnCreateRoomFailedDelegate = (short Error, string Message) =>
        {
            OnCreatedRoomDelegate = null;
            OnCreateRoomFailedDelegate = null;

            Failed(Error, Message);
        };


        bool Result = PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = maxPlayers });

        if (!Result)
        {
            OnCreatedRoomDelegate = null;
            OnCreateRoomFailedDelegate = null;

            Failed(0x6666, "Create room failed");
        }
    }

    public override void OnConnectedToMaster()
    {
        if(logging)
        Debug.Log($"Connected to master");

        PhotonNetwork.JoinLobby();
    }

    public override void OnRoomListUpdate(List<RoomInfo> RoomList)
    {
        if (PhotonNetwork.InRoom)
        {
            return;
        }

        if (logging)
            Debug.Log($"Room list updated");

        foreach (RoomInfo Room in RoomList)
        {
            if (Room.IsOpen)
            {
                if (logging)
                    Debug.Log($"Joining room {Room.Name}");

                if (PhotonNetwork.JoinRoom(Room.Name))
                {
                    return;
                }
            }
        }

        if (logging)
            Debug.Log($"Creating room");

        CreateRoom("MyRoom", () =>
        {
            if (logging)
                Debug.Log($"Room created");

        }, (short Error, string Message) =>
        {
            if (logging)
                Debug.Log($"Room create failed for reason {Message}");
        });
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        if (logging)
            Debug.Log($"Disconnected for reason {cause}");
    }

    public override void OnJoinedLobby()
    {
        if (logging)
            Debug.Log($"Joined lobby");
    }

    public override void OnLeftLobby()
    {
        if (logging)
            Debug.Log($"Left lobby");
    }

    public override void OnCreatedRoom()
    {
        OnCreatedRoomDelegate();
    }

    public override void OnCreateRoomFailed(short ReturnCode, string Message)
    {
        OnCreateRoomFailedDelegate(ReturnCode, Message);
    }

    public override void OnJoinedRoom()
    {
        if (logging)
            Debug.Log($"Joined room");

        OnJoinedRoomDelegate();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        if (logging)
            Debug.Log($"Room join failed for reason {message}");
    }

    public override void OnPlayerEnteredRoom(Player Player)
    {
        if (logging)
            Debug.Log($"Player {Player.NickName} joined the room");

        OnPlayerJoinedDelegate(Player);
    }

    public override void OnPlayerLeftRoom(Player Player)
    {
        if (logging)
            Debug.Log($"Player {Player.NickName} left the room");

        OnPlayerLeftDelegate(Player);
    }

    // Abstract

    protected static GameInstance _Instance;

    public static bool Initialized => _Instance != null;

    public static GameInstance Instance
    {
        get
        {
            if (!Initialized)
            {
                GameObject GameObject = new GameObject("GameInstance");

                _Instance = GameObject.AddComponent<GameInstance>();
            }

            return _Instance;
        }
    }

    [RuntimeInitializeOnLoadMethod]
    static void ForceInit()
    {
        GameInstance GI = Instance;
    }
}
