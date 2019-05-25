using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardEntry : MonoBehaviour
{
    #region Fields

    public Image rankImage;
    public TextMeshProUGUI rankText;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI timeText;

    #endregion

    #region Unity Methods
    public void SetUIValues(Sprite sprite, string rank, HighscoreEntry entry)
    {
        rankImage.sprite = sprite;
        rankText.text = rank;
        nameText.text = entry.name;
        timeText.text = System.TimeSpan.Parse(entry.lapTime).ToString(@"mm\:ss\.ff");
    }
    #endregion
}
