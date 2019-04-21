using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomManager : MonoBehaviour
{
    #region Private Serializable Fields

    [Tooltip("Player lobby labels")]
    [SerializeField]
    private PlayerLabel[] playerLabels;

    #endregion

    public void Awake()
    {
        UpdateShownPlayerList(PhotonNetwork.PlayerList);

        GameInstance.Instance.OnPlayerLeftDelegate = (Player player) =>
        {
            UpdateShownPlayerList(PhotonNetwork.PlayerList);
        };

        GameInstance.Instance.OnPlayerJoinedDelegate = (Player player) =>
        {
            UpdateShownPlayerList(PhotonNetwork.PlayerList);
        };
    }

    public void LoadScene(string mapName)
    {
        PhotonNetwork.LoadLevel(mapName);
    }

    private void UpdateShownPlayerList(Player[] players)
    {
        CleanPlayerList();

        bool nameOnList;
        int i = 0;

        foreach (Player player in players)
        {
            nameOnList = false;

            foreach (PlayerLabel lbl in playerLabels)
            {
                if (nameOnList)
                    break;

                Text textComponent = lbl.GetComponent<Text>();

                if (textComponent.text.Equals(""))
                {
                    if (!nameOnList)
                    {
                        textComponent.text = player.NickName;
                        nameOnList = true;
                    }
                }
            }
        }


    }

    private void CleanPlayerList()
    {
        foreach (PlayerLabel lbl in playerLabels)
        {
            lbl.GetComponent<Text>().text = "";
        }
    }
}