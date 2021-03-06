﻿using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using System.IO;
using Photon.Realtime;
using System.Collections;

public class PlayerNetwork : MonoBehaviourPunCallbacks
{
    public static PlayerNetwork Instance;

    public string PlayerName { get; private set; }
    public string activeScene;

    public ExitGames.Client.Photon.Hashtable playerCustomProperties = new ExitGames.Client.Photon.Hashtable();
    private PhotonView photonView;

    // How many players fully loaded the game scene
    private int playersInGame = 0;
    private PlayerShip playerShip;
    private Coroutine pingCoroutine;
    public int mySpawnIndex = 0;

    public static bool LoadingFromGame = false;

    private void Awake()
    {
        Instance = this;
        photonView = GetComponent<PhotonView>();

        SetAccount();

        // Higher bandwidth cost, smoother movement
        PhotonNetwork.SendRate = 120;
        PhotonNetwork.SerializationRate = 60;
    }

    private void Start()
    {
        // Delegate, when scene loaded method called
        SceneManager.sceneLoaded += OnSceneFinishedLoading;
    }

    public void SetAccount()
    {
        Account account = Registration.GetCurrentAccount();

        PlayerName = account.username + "#" + Random.Range(1000, 9999);

        PhotonNetwork.LocalPlayer.NickName = PlayerName;

        PhotonNetwork.AuthValues = new AuthenticationValues(PlayerName);
        //PlayerName = "Kevin#" + Random.Range(1000, 9999);
    }

    // Called when scene finished loading
    private void OnSceneFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        if (!scene.name.Equals(ScenesInformation.sceneNames[SceneTitle.MAIN]) && !scene.name.Equals(ScenesInformation.sceneNames[SceneTitle.SHIPYARD]))
        {
            if (PhotonNetwork.IsMasterClient)
                MasterLoadedGame();
            else
                NonMasterLoadedGame();
        }
    }

    // Host loaded game
    private void MasterLoadedGame()
    {
        // Bug fix on scene load
        photonView = GetComponent<PhotonView>();

        Scene scene = SceneManager.GetActiveScene();

        // Set that master has loaded the scene
        photonView.RPC("RPC_LoadedGameScene", RpcTarget.MasterClient, PhotonNetwork.LocalPlayer);

        // Tell the clients (other players) load in the game rn
        photonView.RPC("RPC_LoadGameOthers", RpcTarget.Others, scene.name);
    }

    // Client loaded game
    private void NonMasterLoadedGame()
    {
        // Get photonview again - Bug fix on scene load
        photonView = GetComponent<PhotonView>();
        photonView.RPC("RPC_LoadedGameScene", RpcTarget.MasterClient, PhotonNetwork.LocalPlayer);
    }

    // Tell all clients to load the same game as the host
    [PunRPC]
    private void RPC_LoadGameOthers(string sceneName)
    {
        activeScene = sceneName;

        PhotonNetwork.LoadLevel(sceneName);
        CurrentRoomCanvas.instance.StartLoadingScreen();
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

    // Creates the player's ship
    [PunRPC]
    private void RPC_CreatePlayer()
    {
        mySpawnIndex = PlayerLayoutGroup.Instance.GetListingIndex();
        playerShip = PlayerManager.Instance.CreatePlayer(PhotonNetwork.LocalPlayer, activeScene, mySpawnIndex);
    }

    public void ResetNetwork()
    {
        playersInGame = 0;
        playerShip = null;
        activeScene = string.Empty;
    }

    #region health
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
        }

    }
    #endregion

    #region ping
    private IEnumerator C_SetPing()
    {
        while (PhotonNetwork.IsConnected)
        {
            playerCustomProperties["ping"] = PhotonNetwork.GetPing();
            PhotonNetwork.LocalPlayer.SetCustomProperties(playerCustomProperties);

            yield return new WaitForSeconds(2f);
        }

        yield break;
    }

    public override void OnConnectedToMaster()
    {
        if (pingCoroutine == null)
            pingCoroutine = StartCoroutine(C_SetPing());
    }
    #endregion

    public static void ReturnToMain()
    {
        if (PhotonNetwork.InRoom)
            PhotonNetwork.LeaveRoom();

        Instance.ResetNetwork();
        SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
        PhotonNetwork.LoadLevel(ScenesInformation.sceneNames[SceneTitle.MAIN]);

        if (PhotonNetwork.IsConnectedAndReady)
        {
            PhotonNetwork.Disconnect();
        }
    }

    public static void ReturnToLobby()
    {
        ReturnToMain();

        LoadingFromGame = true;

        SceneManager.sceneLoaded += (Scene scene, LoadSceneMode mode) =>
        {
            if (scene.name.Equals(ScenesInformation.sceneNames[SceneTitle.MAIN]))
            {
                MainCanvasManager.instance.ShowPanel(PanelType.LOBBY);
                MainCanvasManager.instance.MainMenu.gameObject.SetActive(false);
            }
        };
    }
}
