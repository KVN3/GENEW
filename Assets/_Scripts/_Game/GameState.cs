using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using UnityEngine;
using UnityEngine.Assertions;

[System.Serializable]
public struct GameManagers
{
    // Analytics
    public AnalyticsManager analyticsManagerClass;

    // Enemies
    public SpawnPointManager spawnPointManagerClass;
    public ChaserManager chaserManagerClass;
    public MoverManager moverManagerClass;
    public AsteroidStormManager asteroidStormManagerClass;

    // Game
    public RaceManager raceManagerClass;
    public UIManager UIManagerClass;

    // Sounds
    public BackgroundSoundManager backgroundSoundManagerClass;
}

[System.Serializable]
public struct AttachableScripts
{
    public Floater floaterScript;
    public Rotator rotatorScript;
}

public class GameState : LevelSingleton<GameState>
{
    public GameManagers gameManagers;

    // Classes
    public PlayerCamera cameraClass;
    public PlayerShip playerClass;

    // Spawnpoints
    public LocalSpawnPoint[] playerStarts;

    // Players
    public Dictionary<Player, PlayerShip> players { get; set; } = new Dictionary<Player, PlayerShip>();

    // Ghosts (replay)
    public PlayerShipGhost[] ghosts;

    // Global variables
    public static bool gamePaused = false;

    // Listeners
    private GameObject[] listeners;

    private bool logging = false;

    protected override void Awake()
    {
        base.Awake();

        Assert.IsNotNull(playerClass);
        Assert.IsFalse(playerStarts.Length == 0);

        GameInstance.Connect();

        GameInstance.Instance.OnJoinedRoomDelegate = () =>
        {
            if (logging)
                Debug.Log("I jkoined the game");

            // Spawn the local player - BAD SOLUTION
            int index = PhotonNetwork.PlayerList.Length - 1; // - 1 to fix for mp
            SpawnLocalPlayer(index);

            // Update player list
            StartCoroutine(UpdatePlayerList());
        };

        GameInstance.Instance.OnPlayerJoinedDelegate = (Player player) => { StartCoroutine(UpdatePlayerList()); };

        GameInstance.Instance.OnPlayerLeftDelegate = (Player player) => { StartCoroutine(UpdatePlayerList()); };
    }

    void Start()
    {

        Assert.IsNotNull(gameManagers.backgroundSoundManagerClass);
        Assert.IsNotNull(gameManagers.chaserManagerClass);
        Assert.IsNotNull(gameManagers.UIManagerClass);
        Assert.IsNotNull(gameManagers.spawnPointManagerClass);
        Assert.IsNotNull(gameManagers.asteroidStormManagerClass);
        Assert.IsNotNull(gameManagers.raceManagerClass);
        Assert.IsNotNull(gameManagers.analyticsManagerClass);


        RaceManager raceManager = Instantiate(gameManagers.raceManagerClass);
        foreach (PlayerShipGhost ghost in ghosts)
            Instantiate(ghost);

        BackgroundSoundManager backgroundSoundManager = Instantiate(gameManagers.backgroundSoundManagerClass);
    }

    #region Photon
    protected PlayerShip SpawnLocalPlayer(int index)
    {
        if (logging)
            Debug.Log("Spawning local player...");

        LocalSpawnPoint playerStart = playerStarts[index];

        PlayerShip playerShip = PhotonNetwork.Instantiate(playerClass.name, playerStart.transform.position, playerStart.transform.rotation).GetComponent<PlayerShip>();

        // UIManager
        UIManager UIManager = Instantiate(gameManagers.UIManagerClass);
        UIManager.playerShip = playerShip;
        UIManager.playerCount = players.Count;

        // Spawn camera
        PlayerCamera camera = Spawn(cameraClass);
        camera.target = playerShip;

        // Camera reference for the PC
        PlayerController playerController = playerShip.gameObject.GetComponent<PlayerController>();
        playerController.SetPlayerCamera(camera);
        playerShip.SetPlayerCamera(camera);

        // Add to list
        players.Add(PhotonNetwork.LocalPlayer, playerShip);

        // Temp SP
        PlayerShip[] playersLocal = new PlayerShip[1];
        playersLocal[0] = playerShip;

        // Analytics
        AnalyticsManager analyticsManager = Instantiate(gameManagers.analyticsManagerClass);
        analyticsManager.playerShip = playerShip;

        // Spawn Point Manager
        SpawnPointManager spawnPointManager = Instantiate(gameManagers.spawnPointManagerClass);

        // Enemy Managers
        ChaserManager chaserManager = Instantiate(gameManagers.chaserManagerClass);
        chaserManager.SetPlayers(playersLocal);
        chaserManager.SetSpawnPoints(spawnPointManager.chaserSpawnPoints);

        MoverManager moverManager = Instantiate(gameManagers.moverManagerClass);
        moverManager.SetPlayers(playersLocal);
        moverManager.SetSpawnPoints(spawnPointManager.movingSpawnPoints);

        return playerShip;
    }

    private IEnumerator UpdatePlayerList()
    {
        yield return new WaitForSeconds(5);

        players = new Dictionary<Player, PlayerShip>();

        GameObject[] playerShipObjects = GameObject.FindGameObjectsWithTag("Ship");

        foreach (GameObject shipObject in playerShipObjects)
        {
            PlayerShip playerShip = shipObject.GetComponent<PlayerShip>();

            PhotonView photonView = playerShip.GetComponent<PhotonView>();
            Player player = photonView.Owner;

            players.Add(player, playerShip);
        }

        if (logging)
            Debug.Log("PLAYER COUNT: " + players.Count);
    }

    protected PlayerShip SpawnRemotePlayer(Player remotePlayer, int index)
    {
        Debug.Log("Spawning remote player...");
        LocalSpawnPoint playerStart = playerStarts[index];

        PlayerShip Player = Spawn(playerClass, (PlayerShip newPlayer) =>
        {
            newPlayer.transform.position = playerStart.transform.position;
            newPlayer.transform.rotation = playerStart.transform.rotation;

            newPlayer.remoteData = remotePlayer;
        });

        players.Add(remotePlayer, Player);

        return Player;
    }

    protected void KillPlayer(Player remotePlayer)
    {
        PlayerShip player = players[remotePlayer];

        Destroy(player);

        players.Remove(remotePlayer);
    }
    #endregion

    private void Update()
    {
        // Restart, gets particle error tho
        if (Input.GetKeyDown(KeyCode.R))
            RestartScene();

        // Exit game
        if (Input.GetKeyDown(KeyCode.Escape))
            ExitGame();
    }

    public void RestartScene()
    {
        //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        //SceneManager.LoadScene("Main Menu");
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}

// dif 0 = 3 sec
// dif 1 = 2.5 sec
// dif 2 = 2 sec
// dif 3 = 1.5 sec
// etc... till 0.5 (vl 5)
// splitted comet ride thru little piece of rock smash u ass 

// Questions;;
// SpawnManager -> AttachbleScripts attach scripts during runtime
// Co-op yes 
// Fb code
// Parent / child inheritence (asteroid, lavaasteroid)
// Endless Runner Sample Game

// Asteroid Storm Manager
//if (difficulty > 0)
//{
//    AsteroidStormManager asteroidStormManager = Instantiate(gameManagers.asteroidStormManagerClass);
//    asteroidStormManager.spawnPointManager = spawnPointManager;
//}