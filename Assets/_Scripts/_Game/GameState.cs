using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

[System.Serializable]
public struct GameManagers
{
    // Analytics
    public AnalyticsManager analyticsManagerClass;

    // Enemies
    public SpawnPointManager spawnPointManagerClass;
    public ChaserManager chaserManagerClass;
    public MoverManager moverManagerClass;
    public CirclerManager circlerManagerClass;

    public AsteroidStormManager asteroidStormManagerClass;

    // Game
    public RaceManager raceManagerClass;
    public ReplayManager replayManagerClass;
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
    public static GameState instance;

    [SerializeField]
    private GameManagers gameManagers;

    // Spawnpoints
    public static LocalSpawnPoint[] playerStarts;

    // Ghosts (replay)
    public PlayerShipGhost[] ghosts;

    // Global variables
    public static bool gamePaused = false;

    private PlayerShip[] playerShips;
    public PlayerShip[] GetPlayerShips()
    {
        return playerShips;
    }

    private bool logging = false;
    public static bool spawnEnemies = true;

    protected override void Awake()
    {
        base.Awake();

        instance = this;
    }

    void Start()
    {
        BackgroundSoundManager backgroundSoundManager = Instantiate(gameManagers.backgroundSoundManagerClass);

        Assert.IsNotNull(gameManagers.backgroundSoundManagerClass);
        //Assert.IsNotNull(gameManagers.chaserManagerClass);
        Assert.IsNotNull(gameManagers.UIManagerClass);
        Assert.IsNotNull(gameManagers.spawnPointManagerClass);
        //Assert.IsNotNull(gameManagers.asteroidStormManagerClass);
        Assert.IsNotNull(gameManagers.raceManagerClass);
        Assert.IsNotNull(gameManagers.replayManagerClass);
        Assert.IsNotNull(gameManagers.analyticsManagerClass);
        Assert.IsNotNull(backgroundSoundManager);

        // Race Manager 
        RaceManager raceManager = Instantiate(gameManagers.raceManagerClass);
        ReplayManager replayManager = Instantiate(gameManagers.replayManagerClass);

        if (PlayerPrefs.HasKey("Ghosts"))
            ghosts = new PlayerShipGhost[PlayerPrefs.GetInt("Ghosts")];

        foreach (PlayerShipGhost ghost in ghosts)
            Instantiate(ghost);

        // Master Client related
        if (PhotonNetwork.IsMasterClient)
            StartCoroutine(C_SpawnManagers());
    }

    // Find all player objects and make a reference
    private IEnumerator C_SpawnManagers()
    {
        int shipsInRoom = PhotonNetwork.PlayerList.Length;

        if (ScenesInformation.IsTutorialScene())
            shipsInRoom += 1;
            

        // Find all player ship game objects first
        GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("Ship");
        while (gameObjects.Length != shipsInRoom)
        {
            gameObjects = GameObject.FindGameObjectsWithTag("Ship");
            yield return new WaitForSeconds(1);
        }

        // Get the playership scripts from the objects
        playerShips = new PlayerShip[gameObjects.Length];
        for (int i = 0; i < gameObjects.Length; i++)
        {
            playerShips[i] = gameObjects[i].GetComponent<PlayerShip>();
        }

        while (!RaceManager.raceStarted)
        {
            yield return new WaitForSeconds(1);
        }

        SpawnManagers();
    }

    // Spawn game managers
    private void SpawnManagers()
    {
        if (!spawnEnemies)
            return;

        // Current scene
        string sceneName = SceneManager.GetActiveScene().name;

        // Spawn Point Manager
        SpawnPointManager spawnPointManager = Instantiate(gameManagers.spawnPointManagerClass);

        // Enemy Managers
        if (gameManagers.chaserManagerClass != null)
        {
            ChaserManager chaserManager = Instantiate(gameManagers.chaserManagerClass);

            // Desired alive enemies at a given moment
            chaserManager.desiredAliveEnemyCount = ScenesInformation.GetDesiredChaserAliveCount(sceneName);

            chaserManager.SetPlayers(playerShips);
            chaserManager.SetSpawnPoints(spawnPointManager.chaserSpawnPoints);
        }

        if (gameManagers.moverManagerClass != null)
        {
            MoverManager moverManager = Instantiate(gameManagers.moverManagerClass);

            // Desired alive enemies at a given moment
            moverManager.desiredAliveEnemyCount = ScenesInformation.GetDesiredShooterAliveCount(sceneName);

            moverManager.SetPlayers(playerShips);
            moverManager.SetSpawnPoints(spawnPointManager.movingSpawnPoints);
        }

        // Asteroid Manager
        if (gameManagers.asteroidStormManagerClass != null)
        {
            AsteroidStormManager asteroidStormManager = Instantiate(gameManagers.asteroidStormManagerClass);
            asteroidStormManager.spawnPointManager = spawnPointManager;
        }
    }

    public void RestartLevel()
    {
        // In case game is paused
        Time.timeScale = 1f;
        SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}