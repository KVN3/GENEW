using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using System.IO;

public class PlayerNetwork : MonoBehaviourPunCallbacks
{
    public static PlayerNetwork Instance;

    public string PlayerName { get; private set; }

    private PhotonView photonView;
    private int playersInGame = 0;

    private string activeScene;

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


    // Host loaded game
    private void MasterLoadedGame()
    {
        activeScene = ScenesInformation.sceneNames[SceneTitle.Wasteland];

        photonView.RPC("RPC_LoadedGameScene", RpcTarget.MasterClient);
        photonView.RPC("RPC_LoadGameOthers", RpcTarget.Others, activeScene);
    }

    // Client loaded game
    private void NonMasterLoadedGame()
    {
        photonView.RPC("RPC_LoadedGameScene", RpcTarget.MasterClient);
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
    private void RPC_LoadedGameScene()
    {
        playersInGame++;

        // When all players in the game, spawn their ships in
        if (playersInGame == PhotonNetwork.PlayerList.Length)
        {
            print("All players loaded game scene.");
            photonView.RPC("RPC_CreatePlayer", RpcTarget.All);
        }

    }

    // Creates the player's ship
    [PunRPC]
    private void RPC_CreatePlayer()
    {
        PlayerShip playerShipClass = LoadPlayerShip();
        PlayerCamera playerCameraClass = LoadPlayerCamera();
        LocalSpawnPoint[] spawnPoints = LoadSpawnPoints();

        string playerPath = Path.Combine("Prefabs", "Player", "PlayerShip");

        // Spawn the local player - BAD SOLUTION
        int index = PhotonNetwork.PlayerList.Length - 1;
        LocalSpawnPoint playerStart = spawnPoints[index];

        PlayerShip playerShip = PhotonNetwork.Instantiate(playerPath, playerStart.transform.position, playerStart.transform.rotation).GetComponent<PlayerShip>();

        // Spawn camera
        PlayerCamera camera = Instantiate(playerCameraClass);
        camera.target = playerShip;

        // Camera reference for the PC
        PlayerController playerController = playerShip.gameObject.GetComponent<PlayerController>();
        playerController.SetPlayerCamera(camera);
        playerShip.SetPlayerCamera(camera);
    }




    private PlayerShip LoadPlayerShip()
    {
        string playerPath = Path.Combine("Prefabs", "Player", "PlayerShip");
        GameObject playerShipObj = Resources.Load(playerPath) as GameObject;

        return playerShipObj.GetComponent<PlayerShip>();
    }

    private PlayerCamera LoadPlayerCamera()
    {
        string cameraPath = Path.Combine("Prefabs", "Player", "PlayerCamera");
        GameObject playerCameraObj = Resources.Load(cameraPath) as GameObject;

        return playerCameraObj.GetComponent<PlayerCamera>();
    }

    private LocalSpawnPoint[] LoadSpawnPoints()
    {
        string spawnpointsPath = Path.Combine("Prefabs", "Spawnpoints", activeScene);

        Object[] objects = Resources.LoadAll(spawnpointsPath);
        LocalSpawnPoint[] spawnPoints = new LocalSpawnPoint[objects.Length];

        int i = 0;
        foreach (Object obj in objects)
        {
            GameObject gameObject = obj as GameObject;
            spawnPoints[i] = gameObject.GetComponent<LocalSpawnPoint>();
            i++;
        }

        return spawnPoints;
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
