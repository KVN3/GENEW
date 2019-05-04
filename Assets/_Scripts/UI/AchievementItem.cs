using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class AchievementItem : MonoBehaviour
{
    #region Fields
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI descriptionText;
    public Slider progressBar;
    public TextMeshProUGUI isCompletedText;
	#endregion
	
	#region Unity Methods
    public void SetText(string title, string description, bool isCompleted, float progress, float progressGoal)
    {
        titleText.text = title;
        descriptionText.text = description;

        progressBar.value = progress / progressGoal * 1;

        if (isCompleted)
            isCompletedText.text = LocalizationManager.GetTextByKey("COMPLETED");
        else
            isCompletedText.text = LocalizationManager.GetTextByKey("UNCOMPLETED");
    }
	#endregion
}
