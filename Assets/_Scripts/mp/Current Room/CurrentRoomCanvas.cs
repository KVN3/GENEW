using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.UI.Dropdown;

public class CurrentRoomCanvas : MonoBehaviour
{
    public static CurrentRoomCanvas instance;

    private SceneTitle _sceneTitle;
    [SerializeField]
    private MapSelection _mapSelection;

    public string RoomName { get; set; }

    // Text components
    public TextMeshProUGUI roomNameText;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI lapsText;
    public TextMeshProUGUI leaderboardText;
    public TextMeshProUGUI chatTitle;
    public TextMeshProUGUI connectingText;
    public TextMeshProUGUI enterChatMessageText;
    public TextMeshProUGUI playersInRoomText;
    
    public TextMeshProUGUI ghostsText;
    
    public TextMeshProUGUI startMatchText;
    public TextMeshProUGUI leaveRoomText;

    // Loading screen
    public TextMeshProUGUI progressText;

    // Input fields
    public TMP_InputField ghostInput;

    // Buttons
    [SerializeField]
    private Button playButton;

    // Dropdowns
    [SerializeField]
    private TMP_Dropdown levelDropdown;

    // Other
    public GameObject loadingScreen;
    public Slider loadingBar;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        _sceneTitle = _mapSelection.GetScene();
    }

    private void Update()
    {
        roomNameText.text = RoomName;
        levelText.text = LocalizationManager.GetTextByKey("LEVEL");
        lapsText.text = LocalizationManager.GetTextByKey("LAPS");
        leaderboardText.text = LocalizationManager.GetTextByKey("LEADERBOARD");
        chatTitle.text = LocalizationManager.GetTextByKey("CHAT");
        connectingText.text = LocalizationManager.GetTextByKey("CONNECTING");
        enterChatMessageText.text = LocalizationManager.GetTextByKey("ENTER_CHAT_MESSAGE");
        playersInRoomText.text = LocalizationManager.GetTextByKey("PLAYERS_IN_ROOM");
        leaveRoomText.text = LocalizationManager.GetTextByKey("LEAVE_ROOM");
        startMatchText.text = LocalizationManager.GetTextByKey("START_MATCH");
    }

    #region SCENE LOADING
    public void OnClickStartDelayed(bool tutorial)
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.CurrentRoom.IsVisible = false;

        if (GameConfiguration.tutorial)
        {
            _sceneTitle = SceneTitle.TUTORIAL;
        }

        string sceneName = ScenesInformation.sceneNames[_sceneTitle];

        PlayerNetwork.Instance.activeScene = sceneName;


        PhotonNetwork.LoadLevel(sceneName);

        StartLoadingScreen();

        if (int.TryParse(ghostInput.text, out int result2))
            PlayerPrefs.SetInt("Ghosts", int.Parse(ghostInput.text));
        else PlayerPrefs.SetInt("Ghosts", 1);

    }

    public void StartLoadingScreen()
    {
        StartCoroutine(LoadAsynchronously());
    }

    IEnumerator LoadAsynchronously()
    {
        float progress = PhotonNetwork.LevelLoadingProgress;

        loadingScreen.SetActive(true);
        loadingScreen.transform.SetAsLastSibling();

        while (progress < 1f)
        {
            progress = PhotonNetwork.LevelLoadingProgress;
            progress = Mathf.Clamp01(progress / .9f);

            loadingBar.value = progress;
            progressText.text = (progress * 100).ToString("F0") + "%";

            yield return null;
        }
    }

    public void UpdateScene()
    {
        _sceneTitle = _mapSelection.GetScene();
    }
    #endregion

    #region BUTTON ACCESS
    // Manage masterclient/cient button access
    public void ManageRoomButtonAccess()
    {
        // Skip if not in a room
        if (!PhotonNetwork.InRoom)
            return;

        // Enable for master, disable for clients
        if (PhotonNetwork.IsMasterClient)
        {
            // Tutorial max 1 player
            string sceneName = ScenesInformation.sceneNames[_sceneTitle];
            if (sceneName == "Tutorial")
            {
                if (PhotonNetwork.PlayerList.Length == 1)
                {
                    playButton.interactable = true;
                }
                else
                {
                    playButton.interactable = false;
                }
            }
            else
            {
                playButton.interactable = true;
            }

            levelDropdown.interactable = true;
            levelDropdown.gameObject.SetActive(true);
        }
        else
        {
            playButton.interactable = false;
            levelDropdown.interactable = false;
            levelDropdown.gameObject.SetActive(false);
        }
    }
    #endregion
}
