using UnityEngine;
using TMPro;

public class FriendPanelManager : MonoBehaviour
{
    #region Fields
    public TextMeshProUGUI openFriendsText;
    public TextMeshProUGUI friendsText;
    public TextMeshProUGUI addFriendText;
    public TextMeshProUGUI friendToAddText;
    #endregion

    #region Unity Methods
    void Update()
    {
        openFriendsText.text = LocalizationManager.GetTextByKey("OPEN_FRIENDS");
        friendsText.text = LocalizationManager.GetTextByKey("FRIENDS");
        addFriendText.text = LocalizationManager.GetTextByKey("ADD_FRIEND");
        friendToAddText.text = LocalizationManager.GetTextByKey("FRIEND_TO_ADD");
    }
	#endregion
}
