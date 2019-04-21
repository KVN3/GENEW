using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameLauncher : MonoBehaviour
{
    #region Private Serializable Fields
    [Tooltip("The Ui Panel to let the user enter name, connect and play")]
    [SerializeField]
    private GameObject controlPanel;

    [Tooltip("The Ui Text to inform the user about the connection progress")]
    [SerializeField]
    private Text feedbackText;

    [Tooltip("The UI Loader Anime")]
    [SerializeField]
    private LoaderAnime loaderAnime;
    #endregion

    public void Awake()
    {
        GameInstance.Instance.OnJoinedRoomDelegate = () =>
        {
            feedbackText.text = "";

            // hide the Play button for visual consistency
            controlPanel.SetActive(false);

            if (loaderAnime != null)
            {
                loaderAnime.StartLoaderAnimation();
            }

            LogFeedback("<Color=Green>OnJoinedRoom</Color> with " + PhotonNetwork.CurrentRoom.PlayerCount + " Player(s)");

            Debug.Log("We load the 'Lobby'");
            PhotonNetwork.LoadLevel("Lobby");
        };
    }

    public void Connect()
    {
        GameInstance.Connect();
    }

    void LogFeedback(string message)
    {
        // we do not assume there is a feedbackText defined.
        if (feedbackText == null)
        {
            return;
        }

        // add new messages as a new line and at the bottom of the log.
        feedbackText.text += System.Environment.NewLine + message;
    }
}
