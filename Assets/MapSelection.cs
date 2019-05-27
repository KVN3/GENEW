using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class MapSelection : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private TMP_Dropdown _dropdownList;
    [SerializeField]
    private List<SceneTitle> scenes;

    PhotonView photonView;

    public string activeSceneName;

    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
    }

    private void Start()
    {
        // Populate dropdownlist
        List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();

        foreach (SceneTitle scene in scenes)
        {
            options.Add(new TMP_Dropdown.OptionData(ScenesInformation.sceneNames[scene]));
        }
        
        _dropdownList.AddOptions(options);
    }

    // Get the selected scene from the dropdownlist
    public SceneTitle GetScene()
    {
        SceneTitle scene = scenes[_dropdownList.value];
        return scene;
    }

    //public void UpdateMapDropDown()
    //{
    //    if (!PhotonNetwork.IsMasterClient)
    //        return;

    //    int index = 0;
    //    SceneTitle scene = scenes[_dropdownList.value];

    //    switch (scene)
    //    {
    //        case SceneTitle.HIGHWAY:
    //            index = 0;
    //            break;
    //        case SceneTitle.WASTELAND:
    //            index = 1;
    //            break;
    //        case SceneTitle.TUTORIAL:
    //            index = 2;
    //            break;
    //    }

    //    photonView.RPC("RPC_UpdateSelectedMap", RpcTarget.All, index);

    //}

    //[PunRPC]
    //private void RPC_UpdateSelectedMap(int index)
    //{
    //    _dropdownList.value = index;
    //}
}
