using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

[System.Serializable]
public struct GameManagers
{
    public AsteroidStormManager asteroidStormManagerClass;
    public SpawnPointManager spawnPointManagerClass;
    public UIManager UIManagerClass;
    public ChaserManager chaserManagerClass;
    public MoverManager MoverManagerClass;

    public BackgroundSoundManager backgroundSoundManagerClass;
}

[System.Serializable]
public struct AttachableScripts
{
    public Floater floaterScript;
    public Rotator rotatorScript;
}

public class GameState : MonoBehaviour
{
    public PlayerShip[] players;

    public GameManagers gameManagers;
    public int difficulty = 0;

    // Listeners
    private GameObject[] listeners;

    void Start()
    {
        Assert.IsNotNull(gameManagers.backgroundSoundManagerClass);
        Assert.IsNotNull(gameManagers.chaserManagerClass);
        Assert.IsNotNull(gameManagers.UIManagerClass);
        Assert.IsNotNull(gameManagers.spawnPointManagerClass);
        Assert.IsNotNull(gameManagers.asteroidStormManagerClass);
        Assert.IsNotNull(players, "playerShips niet geassigned");

        //UIManager
        UIManager UIManager = Instantiate(gameManagers.UIManagerClass);
        UIManager.playerShip = players[0];
        UIManager.playerCount = players.Length;

        // Spawn Point Manager
        SpawnPointManager spawnPointManager = Instantiate(gameManagers.spawnPointManagerClass);

        // Asteroid Storm Manager
        //if (difficulty > 0)
        //{
        //    AsteroidStormManager asteroidStormManager = Instantiate(gameManagers.asteroidStormManagerClass);
        //    asteroidStormManager.spawnPointManager = spawnPointManager;
        //}

        // Enemy Managers
        ChaserManager chaserManager = Instantiate(gameManagers.chaserManagerClass);
        chaserManager.SetPlayers(players);
        chaserManager.SetSpawnPoints(spawnPointManager.chaserSpawnPoints);

        MoverManager moverManager = Instantiate(gameManagers.MoverManagerClass);
        moverManager.SetPlayers(players);
        moverManager.SetSpawnPoints(spawnPointManager.movingSpawnPoints);

        BackgroundSoundManager backgroundSoundManager = Instantiate(gameManagers.backgroundSoundManagerClass);
    }

    private void Update()
    {
        // Adds to the laptime based on Time.DeltaTime (A second / fps)
        if (!players[0].runData.raceFinished)
            players[0].runData.raceTime = players[0].runData.raceTime.Add(System.TimeSpan.FromSeconds(1 * Time.deltaTime));

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
        SceneManager.LoadScene("Main Menu");
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void IncreaseDifficulty()
    {
        difficulty++;
        UpdateDifficultyForListeners();
    }

    private void UpdateDifficultyForListeners()
    {
        foreach (GameObject listener in listeners)
        {
            // Send difficulty
        }
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
