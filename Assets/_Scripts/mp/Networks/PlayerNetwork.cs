using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using System.IO;
using Photon.Realtime;

public class PlayerNetwork : MonoBehaviourPunCallbacks
{
    public static PlayerNetwork Instance;

    public string PlayerName { get; private set; }

    private PhotonView photonView;

    // How many players fully loaded the game scene
    private int playersInGame = 0;

    private PlayerShip playerShip;

    private string activeScene;

    private void Awake()
    {
        Instance = this;
        photonView = GetComponent<PhotonView>();

        PlayerName = "Kevin#" + Random.Range(1000, 9999);

        // Higher bandwidth cost, smoother movement
        PhotonNetwork.SendRate = 60;
        PhotonNetwork.SerializationRate = 30;


    }

    private void Start()
    {
        // Delegate, when scene loaded method called
        SceneManager.sceneLoaded += OnSceneFinishedLoading;
    }


    private void OnSceneFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        if (!scene.name.Equals(ScenesInformation.sceneNames[SceneTitle.Main]) && !scene.name.Equals(ScenesInformation.sceneNames[SceneTitle.Shipyard]))
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

    // Host loaded game
    private void MasterLoadedGame()
    {
        // Bug fix on scene load
        photonView = GetComponent<PhotonView>();

        activeScene = ScenesInformation.sceneNames[SceneTitle.Wasteland];

        photonView.RPC("RPC_LoadedGameScene", RpcTarget.MasterClient, PhotonNetwork.LocalPlayer);
        photonView.RPC("RPC_LoadGameOthers", RpcTarget.Others, activeScene);
    }

    // Client loaded game
    private void NonMasterLoadedGame()
    {
        // Bug fix on scene load
        photonView = GetComponent<PhotonView>();

        photonView.RPC("RPC_LoadedGameScene", RpcTarget.MasterClient, PhotonNetwork.LocalPlayer);
    }

    // Tell all clients to load the same game as the host
    [PunRPC]
    private void RPC_LoadGameOthers(string scene)
    {
        // Same scene as host
        activeScene = scene;

        PhotonNetwork.LoadLevel(activeScene);
    }

    // Executed whenever a player loaded the scene
    [PunRPC]
    private void RPC_LoadedGameScene(Player player)
    {
        // Add to player list
        PlayerManager.Instance.AddPlayerStats(player);

        playersInGame++;

        // When all players in the game, spawn their ships in
        if (playersInGame == PhotonNetwork.PlayerList.Length)
        {
            print("All players loaded game scene.");
            photonView.RPC("RPC_CreatePlayer", RpcTarget.All);
        }
    }

    public void NewHealth(Player player, int health)
    {
        photonView.RPC("RPC_NewHealth", player, health);
    }

    [PunRPC]
    private void RPC_NewHealth(int health)
    {
        if (playerShip == null)
            return;

        print("Current player health: " + health);

        if (health < 1)
        {
            PhotonNetwork.Destroy(playerShip.gameObject);
            //Destroy(playerCamera.gameObject);

            // TO DO: Respawning
        }
        
    }

    // Creates the player's ship
    [PunRPC]
    private void RPC_CreatePlayer()
    {
        playerShip = PlayerManager.Instance.CreatePlayer(PhotonNetwork.LocalPlayer, activeScene);

        SkinManager skinManager = playerShip.GetComponent<SkinManager>();
        ShipSkin skin = new ShipSkin();
        skin.baseColor = PlayerPrefsX.GetColor("REGULAR_COLOR");
        skin.lightColor = PlayerPrefsX.GetColor("LIGHT_COLOR");
        skin.darkColor = PlayerPrefsX.GetColor("DARK_COLOR");

        skinManager.ApplySkin(skin);

    }


    //protected PlayerShip SpawnLocalPlayer(int index)
    //{
    //    Debug.Log("Spawning local player...");

    //    LocalSpawnPoint playerStart = playerStarts[index];

    //    PlayerShip playerShip = PhotonNetwork.Instantiate(playerClass.name, playerStart.transform.position, playerStart.transform.rotation).GetComponent<PlayerShip>();

    //     UIManager
    //    UIManager UIManager = Instantiate(gameManagers.UIManagerClass);
    //    UIManager.playerShip = playerShip;
    //    UIManager.playerCount = players.Count;

    //     Spawn camera
    //    PlayerCamera camera = Spawn(cameraClass);
    //    camera.target = playerShip;

    //     Camera reference for the PC
    //    PlayerController playerController = playerShip.gameObject.GetComponent<PlayerController>();
    //    playerController.SetPlayerCamera(camera);
    //    playerShip.SetPlayerCamera(camera);

    //     Add to list
    //    players.Add(PhotonNetwork.LocalPlayer, playerShip);

    //     Temp SP
    //    PlayerShip[] playersLocal = new PlayerShip[1];
    //    playersLocal[0] = playerShip;

    //     Analytics
    //    AnalyticsManager analyticsManager = Instantiate(gameManagers.analyticsManagerClass);
    //    analyticsManager.playerShip = playerShip;

    //    return playerShip;
    //}
}
