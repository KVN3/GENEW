using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelPreview : MonoBehaviourPunCallbacks
{
    public MapSelection mapSelection;
    public GameObject[] levelPreviewImages;

    public void UpdateLevelPreview()
    {
        

        if (PhotonNetwork.IsMasterClient)
        {
            string sceneName = string.Empty;

            SceneTitle sceneTitle = mapSelection.GetScene();
            sceneName = ScenesInformation.sceneNames[sceneTitle];
            photonView.RPC("RPC_NotifyOthers", RpcTarget.All, sceneName);

            SetLevelActive(sceneName);
        }

        
    }

    public void SetLevelActive(string sceneName)
    {
        foreach (GameObject gameObject in levelPreviewImages)
        {
            if (gameObject.name.Equals(sceneName))
            {
                gameObject.SetActive(true);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
    }

    [PunRPC]
    private void RPC_NotifyOthers(string sceneName)
    {
        SetLevelActive(sceneName);
    }


}
