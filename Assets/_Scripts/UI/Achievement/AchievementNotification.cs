using UnityEngine;
using TMPro;

public class AchievementNotification : MonoBehaviour
{
    #region Fields
	public TextMeshProUGUI titleText;
	#endregion
	
	#region Unity Methods
	public void SetText(Achievement achievement)
    {
        titleText.text = achievement.title;
    }
	#endregion
}
