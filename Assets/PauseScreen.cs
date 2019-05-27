using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class PauseScreen : MonoBehaviour
{
    #region Fields
    public Button restartBtn;
	#endregion
	
	#region Unity Methods
	void Start()
    {
        //if (PhotonNetwork.PlayerList.Length == 1)
        //    restartBtn.gameObject.SetActive(true);
    }
	#endregion
}
