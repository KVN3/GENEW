using UnityEngine;
using TMPro;

public class AchievementMenu : MonoBehaviour
{
    #region Fields
    [Header("Text")]
    public TextMeshProUGUI titleText;
    #endregion

    #region Unity Methods
    void Update()
    {
        titleText.text = LocalizationManager.GetTextByKey("ACHIEVEMENTS");
    }
	#endregion
}
