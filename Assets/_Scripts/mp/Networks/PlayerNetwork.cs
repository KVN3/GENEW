using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class PlayerNetwork : MonoBehaviourPunCallbacks
{
    public static PlayerNetwork Instance;

    public string PlayerName { get; private set; }

    private PhotonView photonView;
    private int playersInGame = 0;

    private void Awake()
    {
        Instance = this;
        photonView = GetComponent<PhotonView>();

        PlayerName = "Kevin#" + Random.Range(1000, 9999);

        // Delegate, when scene loaded method called
        SceneManager.sceneLoaded += OnSceneFinishedLoading;
    }


    private void OnSceneFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        if (!scene.name.Equals("Main Menu"))
        {
            string wasteland = ScenesInformation.sceneNames[SceneTitle.Wasteland];

            if (scene.name.Equals(wasteland))
            {
                if (PhotonNetwork.IsMasterClient)
                    MasterLoadedGame();
                else
                    NonMasterLoadedGame();
            }
        }
    }

    private void MasterLoadedGame()
    {
        playersInGame = 1;
        photonView.RPC("RPC_LoadGameOthers", RpcTarget.Others);
    }

    private void NonMasterLoadedGame()
    {
        photonView.RPC("RPC_LoadedGameScene", RpcTarget.MasterClient);
    }

    [PunRPC]
    private void RPC_LoadGameOthers()
    {
        string wasteland = ScenesInformation.sceneNames[SceneTitle.Wasteland];

        PhotonNetwork.LoadLevel(wasteland);
    }

    [PunRPC]
    private void RPC_LoadedGameScene()
    {
        playersInGame++;
        if (playersInGame == PhotonNetwork.PlayerList.Length)
            print("All players loaded game scene.");
    }
}
