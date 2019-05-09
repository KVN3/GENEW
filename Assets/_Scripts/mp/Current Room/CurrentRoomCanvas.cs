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
    private SceneTitle _sceneTitle;
    [SerializeField]
    private MapSelection _mapSelection;

    public Text roomStateText;
    public Text startMatchText;
    public Text leaveRoomText;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI leaderboardText;
    public TextMeshProUGUI leaderboardTimesText;
    public TextMeshProUGUI lapsText;
    public TMP_InputField lapInput;
    public TextMeshProUGUI ghostsText;
    public TMP_InputField ghostInput;

    private void Start()
    {
        _sceneTitle = _mapSelection.GetScene();
    }

    private void Update()
    {
        leaveRoomText.text = LocalizationManager.GetTextByKey("LEAVE_ROOM");
        startMatchText.text = LocalizationManager.GetTextByKey("START_MATCH");
        levelText.text = LocalizationManager.GetTextByKey("LEVEL");
        roomStateText.text = LocalizationManager.GetTextByKey("PUBLIC_ROOM");
        leaderboardText.text = LocalizationManager.GetTextByKey("LEADERBOARD");

        // Get highscores (and sorts them beforehand)
        List<HighscoreEntry> highscoreEntries = HighscoreManager.Instance.GetHighscoresByStage(ScenesInformation.sceneNames[_sceneTitle]);

        // Only show max of 10 or below
        int entries;
        if (highscoreEntries.Count > 10)
            entries = 10;
        else
            entries = highscoreEntries.Count;

        StringBuilder builder2 = new StringBuilder();
        for (int i = 0; i < entries; i++)
        {
            builder2.Append($"{i + 1}. ").Append(highscoreEntries[i].name).Append(": ").Append(TimeSpan.Parse(highscoreEntries[i].lapTime).ToString(@"mm\:ss\.ff")).AppendLine();
        }
        leaderboardTimesText.text = builder2.ToString();
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

        // Save laps
        if (int.TryParse(lapInput.text, out int result))
            PlayerPrefs.SetInt("Laps", int.Parse(lapInput.text));
        else PlayerPrefs.SetInt("Laps", 1);

        if (int.TryParse(ghostInput.text, out int result2))
            PlayerPrefs.SetInt("Ghosts", int.Parse(ghostInput.text));
        else PlayerPrefs.SetInt("Ghosts", 1);

    }

    public void UpdateScene()
    {
        _sceneTitle = _mapSelection.GetScene();
    }
}
