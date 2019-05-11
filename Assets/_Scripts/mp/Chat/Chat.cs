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

    public string[] channelsToJoinOnConnect; // set in inspector. Demo channels to join automatically.

    public string[] friendsList;

    public int historyLengthToFetch; // set in inspector. Up to a certain degree, previously sent messages can be fetched for context

    public string userName { get; set; }

    private string selectedChannelName; // mainly used for GUI/input

    // UI references
    public RectTransform chatPanel;
    public TMP_InputField inputFieldChat;
    public TextMeshProUGUI currentChannelText;
    public Toggle ChannelToggleToInstantiate; // set in inspector

    public ChatClient chatClient;

    public GameObject connectingLabel;

    public GameObject friendListUiItemtoInstantiate;

    private readonly Dictionary<string, Toggle> channelToggles = new Dictionary<string, Toggle>();

    private readonly Dictionary<string, FriendItem> friendListItemLUT = new Dictionary<string, FriendItem>();

    public bool showState = true;
    //public GameObject title;
    public TextMeshProUGUI stateText; // set in inspector
    public TextMeshProUGUI userIdText; // set in inspector
    #endregion

    #region Unity Methods
    public void Start()
    {
        userIdText.text = "";
        stateText.text = "";
        stateText.gameObject.SetActive(true);
        userIdText.gameObject.SetActive(true);
        connectingLabel.SetActive(false);

        Account account = Registration.GetCurrentAccount();
        userName = account.username;

        if (string.IsNullOrEmpty(userName))
            userName = "[guest]" + Environment.UserName;

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

    public void SendChatMessage(string inputLine)
    {
        if (string.IsNullOrEmpty(inputLine))
            return;

        chatClient.PublishMessage(selectedChannelName, inputLine);
    }

    public void StartChat()
    {
        Connect();
    }

    public void Connect()
    {
        chatClient = new ChatClient(this);
        chatClient.Connect(PhotonNetwork.PhotonServerSettings.AppSettings.AppIdChat, "0.0.0", new AuthenticationValues(userName)); //this.chatAppSettings.AppIdChat

        ChannelToggleToInstantiate.gameObject.SetActive(false);
        Debug.Log("Connecting as: " + userName);

        connectingLabel.SetActive(true);
    }

    public void Disconnect()
    {
        if (chatClient != null)
            chatClient.Disconnect();
    }

    public virtual void OnConnected()
    {
        print("Connected to PhotonChat");

        // Subscribe
        if (channelsToJoinOnConnect != null && channelsToJoinOnConnect.Length > 0)
            chatClient.Subscribe(channelsToJoinOnConnect, historyLengthToFetch);

        connectingLabel.SetActive(false);

        userIdText.text = "Connected as " + userName;

        chatPanel.gameObject.SetActive(true);

        if (friendsList != null && friendsList.Length > 0)
        {
            chatClient.AddFriends(friendsList); // Add some users to the server-list to get their status updates

            // add to the UI as well
            foreach (string _friend in friendsList)
            {
                if (friendListUiItemtoInstantiate != null && _friend != userName)
                    InstantiateFriendButton(_friend);
            }
        }

        if (friendListUiItemtoInstantiate != null)
            friendListUiItemtoInstantiate.SetActive(false);


        chatClient.SetOnlineStatus(ChatUserStatus.Online); // You can set your online state (without a mesage).
    }

    public virtual void OnDisconnected()
    {
        print("Disconnected from PhotonChat");
        connectingLabel.SetActive(false);
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
            chatClient.PublishMessage(channel, $"{userName} has joined"); // you don't HAVE to send a msg on join but you could.

            if (ChannelToggleToInstantiate != null)
                InstantiateChannelButton(channel);
        }

        // Switch to the first newly created channel
        ShowChannel(channels[0]);
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

    private void InstantiateFriendButton(string friendId)
    {
        GameObject fbtn = (GameObject)Instantiate(friendListUiItemtoInstantiate);
        fbtn.gameObject.SetActive(true);
        FriendItem _friendItem = fbtn.GetComponent<FriendItem>();

        _friendItem.FriendId = friendId;

        fbtn.transform.SetParent(friendListUiItemtoInstantiate.transform.parent, false);

        friendListItemLUT[friendId] = _friendItem;
    }

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

    public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
    {
        Debug.LogWarning("status: " + string.Format("{0} is {1}. Msg:{2}", user, status, message));

        if (friendListItemLUT.ContainsKey(user))
        {
            FriendItem _friendItem = friendListItemLUT[user];
            if (_friendItem != null) _friendItem.OnFriendStatusUpdate(status, gotMessage, message);
        }
    }

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