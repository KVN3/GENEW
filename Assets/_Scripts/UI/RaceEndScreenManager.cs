using UnityEngine.Assertions;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RaceEndScreenManager : MonoBehaviour
{
    #region Fields & Properties
    public PlayerShip PlayerShip { get; set; }

    public TextMeshProUGUI raceEndScreenText;
    public TextMeshProUGUI endTimeText;
    public TextMeshProUGUI leaderboardText;
    public GameObject entryTemplate;
    public GameObject leaderboardContent;
    public TextMeshProUGUI spectatingText;
    public TextMeshProUGUI rankText;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI toLobbyHelpText;
    public TextMeshProUGUI returnToLobbyText; 
    #endregion

    #region Unity Methods
    private void Awake()
    {
        Assert.IsNotNull(PlayerShip, "No playership in RaceEndScreenManager");
    }

    void Start()
    {
        // Title
        raceEndScreenText.text = LocalizationManager.GetTextByKey("RACE_RESULTS");

        endTimeText.text = LocalizationManager.GetTextByKey("TOTAL_TIME") + ": " + PlayerShip.runData.raceTime.ToString(@"mm\:ss\.ff");

        // Leaderboard
        leaderboardText.text = LocalizationManager.GetTextByKey("LEADERBOARD");

        HighscoreManager.Instance.ShowHighscores(SceneManager.GetActiveScene().name, entryTemplate, leaderboardContent);

        spectatingText.text = LocalizationManager.GetTextByKey("SPECTATING_NEXT");
        rankText.text = LocalizationManager.GetTextByKey("RANK");
        nameText.text = LocalizationManager.GetTextByKey("NAME");
        timeText.text = LocalizationManager.GetTextByKey("TIME");
        toLobbyHelpText.text = LocalizationManager.GetTextByKey("TO_LOBBY_HELP");
        returnToLobbyText.text = LocalizationManager.GetTextByKey("TO_LOBBY");
    }
	#endregion
}
