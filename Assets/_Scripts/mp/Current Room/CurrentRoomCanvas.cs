using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.UI.Dropdown;

public class CurrentRoomCanvas : MonoBehaviour
{
    private SceneTitle _sceneTitle;
    [SerializeField]
    private MapSelection _mapSelection;

    private void Start()
    {
        _sceneTitle = SceneTitle.Test;
    }

    public void OnClickStartDelayed()
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.CurrentRoom.IsVisible = false;

        string sceneName = ScenesInformation.sceneNames[_sceneTitle];

        PlayerNetwork.Instance.activeScene = sceneName;

        PhotonNetwork.LoadLevel(sceneName);
    }

    public void UpdateScene()
    {
        _sceneTitle = _mapSelection.GetScene();
    }
}
