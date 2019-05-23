using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using Photon.Realtime;
using TMPro;

public class PlayerListing : MonoBehaviour
{
    public Player PhotonPlayer { get; private set; }

    [SerializeField]
    private TextMeshProUGUI _playerName;
    [SerializeField]
    private TextMeshProUGUI _playerPing;


    public void ApplyPhotonPlayer(Player photonPlayer)
    {
        PhotonPlayer = photonPlayer;
        _playerName.text = photonPlayer.NickName;

        StartCoroutine(C_ShowPing());
    }

    #region PING
    private IEnumerator C_ShowPing()
    {
        while (PhotonNetwork.IsConnected)
        {
            int ping = (int)PhotonNetwork.LocalPlayer.CustomProperties["ping"];
            _playerPing.text = ping + "ms";

            yield return new WaitForSeconds(1f);
        }

        yield break;
    }
    #endregion
}
