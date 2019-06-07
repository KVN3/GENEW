using UnityEngine;
using TMPro;

public class LoadingScreenManager : MonoBehaviour
{
    #region Fields
    public TextMeshProUGUI loadingText;
	#endregion
	
	#region Unity Methods
    void Update()
    {
        // Translation
        loadingText.text = LocalizationManager.GetTextByKey("LOADING");
    }
	#endregion
}
