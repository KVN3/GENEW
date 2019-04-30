using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrentRoomCanvas : MonoBehaviour
{ 
    public void OnClickStartSync()
    {
        PhotonNetwork.LoadLevel("Large Wasteland");
    }

    public void OnClickDelayedSync()
    {
        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.CurrentRoom.IsVisible = false;

        string wasteland = ScenesInformation.sceneNames[SceneTitle.Wasteland];

        PhotonNetwork.LoadLevel(wasteland);
    }
}
