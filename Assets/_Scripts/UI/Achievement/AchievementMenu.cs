using UnityEngine;
using TMPro;

public class AchievementMenu : MonoBehaviour
{
    #region Fields
    [Header("Text")]
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI returnText;
    #endregion

    #region Unity Methods
    void Update()
    {
        titleText.text = LocalizationManager.GetTextByKey("ACHIEVEMENTS");
        returnText.text = LocalizationManager.GetTextByKey("RETURN");
    }
	#endregion
}
