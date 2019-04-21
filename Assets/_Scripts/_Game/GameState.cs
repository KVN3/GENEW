using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

[System.Serializable]
public struct GameManagers
{
    public AsteroidStormManager asteroidStormManagerClass;
    public SpawnPointManager spawnPointManagerClass;
    public UIManager UIManagerClass;
    public ChaserManager chaserManagerClass;
    public MoverManager moverManagerClass;
    public RaceManager raceManagerClass;

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
    public PlayerShip playerClass;
    public LocalSpawnPoint[] playerStarts;

    // Players
    public Dictionary<Player, PlayerShip> players { get; } = new Dictionary<Player, PlayerShip>();

    // Global variables
    public static bool gamePaused = false;

    // Listeners
    private GameObject[] listeners;

    protected override void Awake()
    {
        base.Awake();

        Assert.IsNotNull(playerClass);
        Assert.IsFalse(playerStarts.Length == 0);

        //GameInstance.Instance.OnJoinedRoomDelegate = () =>
        //{
        //    int PlayerStartIndex = Random.Range(0, playerStarts.Length);

        //    SpawnLocalPlayer(PlayerStartIndex);
        //};
    }

    void Start()
    {

        Assert.IsNotNull(gameManagers.backgroundSoundManagerClass);
        Assert.IsNotNull(gameManagers.chaserManagerClass);
        Assert.IsNotNull(gameManagers.UIManagerClass);
        Assert.IsNotNull(gameManagers.spawnPointManagerClass);
        Assert.IsNotNull(gameManagers.asteroidStormManagerClass);
        Assert.IsNotNull(gameManagers.raceManagerClass);

        // Spawn Point Manager
        SpawnPointManager spawnPointManager = Instantiate(gameManagers.spawnPointManagerClass);

        // Enemy Managers
        //ChaserManager chaserManager = Instantiate(gameManagers.chaserManagerClass);
        //chaserManager.SetPlayers(players);
        //chaserManager.SetSpawnPoints(spawnPointManager.chaserSpawnPoints);

        //MoverManager moverManager = Instantiate(gameManagers.moverManagerClass);
        //moverManager.SetPlayers(players);
        //moverManager.SetSpawnPoints(spawnPointManager.movingSpawnPoints);

        RaceManager raceManager = Instantiate(gameManagers.raceManagerClass);

        BackgroundSoundManager backgroundSoundManager = Instantiate(gameManagers.backgroundSoundManagerClass);
    }

    #region Photon
    protected PlayerShip SpawnLocalPlayer(int index)
    {
        LocalSpawnPoint playerStart = playerStarts[index];

        PlayerShip player = PhotonNetwork.Instantiate(playerClass.name, playerStart.transform.position, playerStart.transform.rotation).GetComponent<PlayerShip>();

        //UIManager
        UIManager UIManager = Instantiate(gameManagers.UIManagerClass);
        UIManager.playerShip = player;
        UIManager.playerCount = players.Count;

        players.Add(PhotonNetwork.LocalPlayer, player);

        return player;
    }

    protected PlayerShip SpawnRemotePlayer(Player remotePlayer, int index)
    {
        LocalSpawnPoint playerStart = playerStarts[index];

        PlayerShip Player = Spawn(playerClass, (PlayerShip newPlayer) => {
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
        // Code should be in UI manager / HUD
        // Adds to the laptime based on Time.DeltaTime (A second / fps)
        //if (!players[0].runData.raceFinished && RaceManager.raceStarted)
        //    players[0].runData.raceTime = players[0].runData.raceTime.Add(System.TimeSpan.FromSeconds(1 * Time.deltaTime));

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