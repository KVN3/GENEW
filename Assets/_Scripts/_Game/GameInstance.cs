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
    #region Public Fields
    [Tooltip("The Ui Panel to let the user enter name, connect and play")]
    [SerializeField]
    private GameObject controlPanel;

    [Tooltip("The UI Label to inform the user that the connection is in progress")]
    [SerializeField]
    private GameObject progressLabel;
    #endregion

    public UnityAction OnJoinedLobbyDelegate;
    public UnityAction OnJoinedRoomDelegate;

    public UnityAction<Player> OnPlayerJoinedDelegate;
    public UnityAction<Player> OnPlayerLeftDelegate;

    private UnityAction OnCreatedRoomDelegate;
    private UnityAction<short, string> OnCreateRoomFailedDelegate;

    void Awake()
    {
        DontDestroyOnLoad(this);
    }

    void Start()
    {
    }

    public static void Connect()
    {
        bool Success = PhotonNetwork.ConnectUsingSettings();

        if (!Success)
        {
            Debug.Log($"Failed connecting to Photon");
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

        bool Result = PhotonNetwork.CreateRoom(Name);

        if (!Result)
        {
            OnCreatedRoomDelegate = null;
            OnCreateRoomFailedDelegate = null;

            Failed(0x6666, "Create room failed");
        }
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log($"Connected to master");

        PhotonNetwork.JoinLobby();
    }

    public override void OnRoomListUpdate(List<RoomInfo> RoomList)
    {
        if (PhotonNetwork.InRoom)
        {
            return;
        }

        Debug.Log($"Room list updated");

        foreach (RoomInfo Room in RoomList)
        {
            if (Room.IsOpen)
            {
                Debug.Log($"Joining room {Room.Name}");

                if (PhotonNetwork.JoinRoom(Room.Name))
                {
                    return;
                }
            }
        }

        Debug.Log($"Creating room");

        CreateRoom("MyRoom", () =>
        {
            Debug.Log($"Room created");

        }, (short Error, string Message) =>
        {
            Debug.Log($"Room create failed for reason {Message}");
        });
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log($"Disconnected for reason {cause}");

        progressLabel.SetActive(false);
        controlPanel.SetActive(true);
    }

    public override void OnJoinedLobby()
    {
        Debug.Log($"Joined lobby");
    }

    public override void OnLeftLobby()
    {
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
        Debug.Log($"Joined room");

        OnJoinedRoomDelegate();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log($"Room join failed for reason {message}");
    }

    public override void OnPlayerEnteredRoom(Player Player)
    {
        Debug.Log($"Player {Player.NickName} joined the room");

        OnPlayerJoinedDelegate(Player);
    }

    public override void OnPlayerLeftRoom(Player Player)
    {
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
