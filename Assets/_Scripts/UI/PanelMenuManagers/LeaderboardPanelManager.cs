using TMPro;
using UnityEngine;

public class LeaderboardPanelManager : MonoBehaviour
{
    #region Fields
    public TextMeshProUGUI leaderboardTitle;
    public TextMeshProUGUI rankText;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI timeText;
    #endregion

    #region Unity Methods
    void Update()
    {
        leaderboardTitle.text = LocalizationManager.GetTextByKey("LEADERBOARD");
        rankText.text = LocalizationManager.GetTextByKey("RANK");
        nameText.text = LocalizationManager.GetTextByKey("NAME");
        timeText.text = LocalizationManager.GetTextByKey("TIME");
    }
	#endregion
}
