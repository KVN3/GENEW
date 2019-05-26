using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using Photon.Chat;
using Photon.Pun;
using System;
using System.Collections.Generic;

public class Chat : MonoBehaviour, IChatClientListener
{
    #region Fields
    public static Chat instance;

    public ChatClient chatClient;

    public List<string> channelsToJoinOnConnect { get; private set; }

    // Friend stuff
    public TMP_InputField addFriendInput;
    public TextMeshProUGUI emptyFriendListText;
    private List<string> friendsList; // Friend list from account.friendList
    private List<GameObject> friendObjects; // List of prefabs instantiated so you can delete them
    public GameObject friendListUiItemtoInstantiate; // Prefab
    private readonly Dictionary<string, Friend> friendListItemLUT = new Dictionary<string, Friend>();

    public int historyLengthToFetch; // set in inspector. Up to a certain degree, previously sent messages can be fetched for context

    public string userName { get; set; }

    private string selectedChannelName; // mainly used for GUI/input

    // UI references
    public RectTransform chatPanel;
    public TMP_InputField inputFieldChat;
    public TextMeshProUGUI currentChannelText;
    public Toggle ChannelToggleToInstantiate; // set in inspector

    public GameObject connectingLabel;

    private readonly Dictionary<string, Toggle> channelToggles = new Dictionary<string, Toggle>();

    public bool showState = true;
    public TextMeshProUGUI stateText; // set in inspector
    public TextMeshProUGUI userIdText; // set in inspector
    #endregion

    #region Unity Methods
    private void Awake()
    {
        instance = this;
    }

    public void Start()
    {
        userIdText.text = "";
        stateText.text = "";
        stateText.gameObject.SetActive(true);
        userIdText.gameObject.SetActive(true);
        connectingLabel.SetActive(false);

        friendObjects = new List<GameObject>();

        // Sets username, loads and sets friend data
        LoadAccountData();

        if (string.IsNullOrEmpty(userName))
            userName = "[guest]" + Environment.UserName;


        channelsToJoinOnConnect = new List<string> { "Global", "Lobby" };

        Connect();

        //#if PHOTON_UNITY_NETWORKING
        //chatAppSettings = PhotonNetwork.PhotonServerSettings.AppSettings;
        //#endif
    }

    private void Update()
    {
        if (chatClient != null)
            chatClient.Service();
        // check if we are missing context, which means we got kicked out to get back to the Photon Demo hub.
        if (stateText == null)
        {
            Destroy(gameObject);
            return;
        }

        stateText.gameObject.SetActive(showState); // this could be handled more elegantly, but for the demo it's ok.
    }

    public void JoinChat(string roomName)
    {
        // Add chat channel
        //if (!chatChannels.Contains(roomName))
        //    chatChannels.Add(roomName);

        // Add to list, subscribe to it and set roomName
        channelsToJoinOnConnect.Add(roomName);
        chatClient.Subscribe(roomName, historyLengthToFetch);
        CurrentRoomCanvas.instance.RoomName = roomName;

        foreach (string s in chatClient.PublicChannels.Keys)
            Debug.Log(s);

        //chatClient.PublicChannels[roomName].ClearMessages(); // Does not work

        //Connect();
    }

    public void LeaveChat(string roomName)
    {
        if (channelsToJoinOnConnect.Contains(roomName))
        {
            string[] channels = { roomName };
            chatClient.Unsubscribe(channels);
            channelsToJoinOnConnect.Remove(roomName);
        }
    }

    public void Connect()
    {
        chatClient = new ChatClient(this);
        chatClient.Connect(PhotonNetwork.PhotonServerSettings.AppSettings.AppIdChat, "0.0.0", new AuthenticationValues(userName)); //this.chatAppSettings.AppIdChat

        // Refresh accountdata
        LoadAccountData();
        CheckFriendAmount();

        ChannelToggleToInstantiate.gameObject.SetActive(false);
        Debug.Log("Connecting as: " + userName);

        connectingLabel.SetActive(true);
    }

    public void Disconnect()
    {
        if (chatClient != null)
            chatClient.Disconnect();
    }

    private void LoadAccountData()
    {
        Account account = Registration.GetCurrentAccount();
        userName = account.username;
        friendsList = account.friendList;
    }

    public virtual void OnConnected()
    {
        print("Connected to PhotonChat");

        // Subscribe
        if (channelsToJoinOnConnect != null && channelsToJoinOnConnect.Count > 0)
            chatClient.Subscribe(channelsToJoinOnConnect.ToArray(), historyLengthToFetch);

        connectingLabel.SetActive(false);

        // Username (debugging)
        userIdText.text = "Connected as " + userName;
        //Debug.Log(userName + ": " + PhotonNetwork.LocalPlayer.UserId);
        //Debug.Log(userName + ": " + chatClient.UserId); 

        DeleteFriendItems();
        CreateFriendItems();


        chatClient.SetOnlineStatus(ChatUserStatus.Online); // You can set your online state (without a mesage).
    }

    public virtual void OnDisconnected()
    {
        print("Disconnected from PhotonChat");
        connectingLabel.SetActive(false);
    }

    #region Channels and messages
    public void SendChatMessage(string inputLine)
    {
        if (string.IsNullOrEmpty(inputLine))
            return;

        chatClient.PublishMessage(selectedChannelName, inputLine);
    }

    public virtual void OnGetMessages(string channelName, string[] senders, object[] messages)
    {
        if (channelName.Equals(selectedChannelName))
        {
            // update text
            ShowChannel(selectedChannelName);
        }
    }

    public void OnSubscribed(string[] channels, bool[] results)
    {
        // in this demo, we simply send a message into each channel. This is NOT a must have!
        foreach (string channel in channels)
        {
            chatClient.PublishMessage(channel, $" has joined"); // you don't HAVE to send a msg on join but you could.

            if (ChannelToggleToInstantiate != null)
                InstantiateChannelButton(channel);
        }

        // Switch to the first newly created channel
        ShowChannel(channels[0]);
    }

    public void ShowChannel(string channelName)
    {
        if (string.IsNullOrEmpty(channelName))
        {
            return;
        }

        ChatChannel channel = null;
        bool found = chatClient.TryGetChannel(channelName, out channel);
        if (!found)
        {
            Debug.Log("ShowChannel failed to find channel: " + channelName);
            return;
        }

        selectedChannelName = channelName;
        currentChannelText.text = channel.ToStringMessages();

        Debug.Log("ShowChannel: " + selectedChannelName);

        foreach (KeyValuePair<string, Toggle> pair in this.channelToggles)
        {
            pair.Value.isOn = pair.Key == channelName ? true : false;
        }
    }

    private void InstantiateChannelButton(string channelName)
    {
        if (channelToggles.ContainsKey(channelName))
        {
            Debug.Log("Skipping creation for an existing channel toggle.");
            return;
        }

        Toggle cbtn = (Toggle)Instantiate(ChannelToggleToInstantiate);
        cbtn.gameObject.SetActive(true);
        cbtn.GetComponentInChildren<ChannelSelector>().SetChannel(channelName);
        cbtn.transform.SetParent(this.ChannelToggleToInstantiate.transform.parent, false);

        channelToggles.Add(channelName, cbtn);
    }
    #endregion

    /// <summary>
    /// New status of another user (you get updates for users set in your friends list).
    /// </summary>
    /// <param name="user">Name of the user.</param>
    /// <param name="status">New status of that user.</param>
    /// <param name="gotMessage">True if the status contains a message you should cache locally. False: This status update does not include a
    /// message (keep any you have).</param>
    /// <param name="message">Message that user set.</param>
    public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
    {

        Debug.LogWarning("status: " + string.Format("{0} is {1}. Msg:{2}", user, status, message));

        if (this.friendListItemLUT.ContainsKey(user))
        {
            Friend _friendItem = friendListItemLUT[user];
            if (_friendItem != null) _friendItem.OnFriendStatusUpdate(status, gotMessage, message);
        }
    }

    #region Friend functions

    private void CreateFriendItems()
    {
        if (friendsList != null && friendsList.Count > 0)
        {
            chatClient.AddFriends(friendsList.ToArray()); // Add some users to the server-list to get their status updates

            // add to the UI as well
            foreach (string _friend in friendsList)
            {
                if (friendListUiItemtoInstantiate != null && _friend != userName)
                    InstantiateFriendButton(_friend);
            }
        }

        if (friendListUiItemtoInstantiate != null)
            friendListUiItemtoInstantiate.SetActive(false);
    }

    private void DeleteFriendItems()
    {
        foreach (GameObject obj in friendObjects)
        {
            Destroy(obj);
        }
        friendObjects.Clear();
    }


    private void InstantiateFriendButton(string friendId)
    {
        GameObject fbtn = (GameObject)Instantiate(friendListUiItemtoInstantiate);
        friendObjects.Add(fbtn);

        // Add onClick event for removeFriend
        fbtn.GetComponent<Button>().onClick.AddListener(delegate { RemoveFriend(friendId); });

        fbtn.gameObject.SetActive(true);
        Friend _friendItem = fbtn.GetComponent<Friend>();

        _friendItem.FriendId = friendId;

        fbtn.transform.SetParent(friendListUiItemtoInstantiate.transform.parent, false);

        friendListItemLUT[friendId] = _friendItem;
    }

    public void AddFriend(string friendId)
    {
        if (!friendsList.Contains(friendId))
        {
            friendsList.Add(friendId);
            Account account = Registration.GetCurrentAccount();
            account.friendList.Add(friendId);

            UpdateFriends(account);
        }
    }

    public void AddFriendByInput()
    {
        string friendToAdd = addFriendInput.text;
        if (!friendsList.Contains(friendToAdd))
        {
            friendsList.Add(friendToAdd);
            Account account = Registration.GetCurrentAccount();
            account.friendList.Add(friendToAdd);

            UpdateFriends(account);
            addFriendInput.text = "";
        }
    }

    public void RemoveFriend(string friendId)
    {
        if (friendsList.Contains(friendId))
        {
            // Chat friendsList
            friendsList.Remove(friendId);
            Account account = Registration.GetCurrentAccount();
            account.friendList.Remove(friendId);
            UpdateFriends(account);
        }
    }

    private void UpdateFriends(Account account)
    {
        CheckFriendAmount();

        // Save to account, refresh by deleting and creating friend items
        Registration.SaveAccount(account);
        DeleteFriendItems();
        CreateFriendItems();
    }
    
    private void CheckFriendAmount()
    {
        if (friendsList.Count > 0)
            emptyFriendListText.gameObject.SetActive(false);
        else
            emptyFriendListText.gameObject.SetActive(true);
    }

    #endregion

    #region OnInput
    public void OnEnterSend()
    {
        if (Input.GetKey(KeyCode.Return) || Input.GetKey(KeyCode.KeypadEnter))
        {
            SendChatMessage(inputFieldChat.text);
            inputFieldChat.text = "";
        }
    }

    public void OnClickSend()
    {
        if (inputFieldChat != null)
        {
            SendChatMessage(inputFieldChat.text);
            inputFieldChat.text = "";
        }
    }
    #endregion

    #region Debug
    public void OnUserSubscribed(string channel, string user)
    {
        Debug.LogFormat("OnUserSubscribed: channel=\"{0}\" userId=\"{1}\"", channel, user);
    }

    public void OnUserUnsubscribed(string channel, string user)
    {
        Debug.LogFormat("OnUserUnsubscribed: channel=\"{0}\" userId=\"{1}\"", channel, user);
    }

    public void DebugReturn(ExitGames.Client.Photon.DebugLevel level, string message)
    {
        if (level == ExitGames.Client.Photon.DebugLevel.ERROR)
            Debug.LogError(message);
        else if (level == ExitGames.Client.Photon.DebugLevel.WARNING)
            Debug.LogWarning(message);
        else
            Debug.Log(message);
    }
    #endregion

    #region Dashboard
    [ContextMenu("OpenDashboard")]
    public void OpenDashboard()
    {
        Application.OpenURL("https://dashboard.photonengine.com");
    }
    #endregion

    #region OnDestroy/Quit
    /// <summary>To avoid that the Editor becomes unresponsive, disconnect all Photon connections in OnDestroy.</summary>
    public void OnDestroy()
    {
        if (chatClient != null)
        {
            chatClient.Disconnect();
        }
    }

    /// <summary>To avoid that the Editor becomes unresponsive, disconnect all Photon connections in OnApplicationQuit.</summary>
    public void OnApplicationQuit()
    {
        if (chatClient != null)
        {
            chatClient.Disconnect();
        }
    }
    #endregion

    #region Unused methods
    // Unused methods

    public void OnPrivateMessage(string sender, object message, string channelName)
    {

    }

    public void OnUnsubscribed(string[] channels)
    {

    }

    public void OnChatStateChange(ChatState state)
    {

    }
    #endregion
}
#endregion