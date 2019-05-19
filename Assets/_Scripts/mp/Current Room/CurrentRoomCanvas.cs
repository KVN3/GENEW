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

    public TextMeshProUGUI roomNameText;
    public TextMeshProUGUI startMatchText;
    public TextMeshProUGUI leaveRoomText;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI leaderboardText;
    public TextMeshProUGUI leaderboardTimesText;
    public TextMeshProUGUI lapsText;
    public TMP_InputField lapInput;
    public TextMeshProUGUI ghostsText;
    public TMP_InputField ghostInput;

    public GameObject loadingScreen;
    public Slider loadingBar;
    public TextMeshProUGUI progressText;

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
        leaveRoomText.text = LocalizationManager.GetTextByKey("LEAVE_ROOM");
        startMatchText.text = LocalizationManager.GetTextByKey("START_MATCH");
        levelText.text = LocalizationManager.GetTextByKey("LEVEL");
        leaderboardText.text = LocalizationManager.GetTextByKey("LEADERBOARD");
    }

    public void OnClickStartDelayed()
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.CurrentRoom.IsVisible = false;

        string sceneName = ScenesInformation.sceneNames[_sceneTitle];

        PlayerNetwork.Instance.activeScene = sceneName;

        PhotonNetwork.LoadLevel(sceneName);
        StartCoroutine(LoadAsynchronously());

        // Save laps
        if (int.TryParse(lapInput.text, out int result))
            PlayerPrefs.SetInt("Laps", int.Parse(lapInput.text));
        else PlayerPrefs.SetInt("Laps", 1);

        if (int.TryParse(ghostInput.text, out int result2))
            PlayerPrefs.SetInt("Ghosts", int.Parse(ghostInput.text));
        else PlayerPrefs.SetInt("Ghosts", 1);

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

            Debug.Log(progress);
            
            loadingBar.value = progress;
            progressText.text = (progress * 100).ToString("F0") + "%";

            yield return null;
        }
    }

    public void UpdateScene()
    {
        _sceneTitle = _mapSelection.GetScene();
    }
}
