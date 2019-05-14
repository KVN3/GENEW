using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance;
    private PhotonView photonView;

    private List<PlayerStats> playerStatsList = new List<PlayerStats>();

    public List<PlayerStats> GetPlayerStatsList()
    {
        return playerStatsList;
    }

    private void Awake()
    {
        Instance = this;
        photonView = GetComponent<PhotonView>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            print("Players in list: " + playerStatsList.Count);
        }
    }

    // Adds PlayerStats to list if it hasn't been added yet
    public void AddPlayerStats(Player player)
    {
        int index = playerStatsList.FindIndex(i => i.player == player);

        if (!Method.IndexFound(index))
        {
            playerStatsList.Add(new PlayerStats(player, 100));
        }
    }

    // Modify ship health
    public void ModifyHealth(Player player, int value)
    {
        int index = playerStatsList.FindIndex(i => i.player == player);

        if (Method.IndexFound(index))
        {
            PlayerStats playerStats = playerStatsList[index];
            playerStats.shipHealth += value;
            PlayerNetwork.Instance.NewHealth(player, playerStats.shipHealth);
        }
    }

    // Instantiates the player's ship
    public PlayerShip CreatePlayer(Player player, string activeScene)
    {
        string playerPath = Path.Combine("Prefabs", "Player", "PlayerShip");
        PlayerCamera playerCameraClass = SharedResources.LoadPlayerCamera();
        RadarCamera radarCameraClass = SharedResources.LoadRadarCamera();

        UIManager playerUIClass = SharedResources.LoadUI();
        AnalyticsManager analyticsManagerClass = SharedResources.LoadAnalyticsManager();
        LocalSpawnPoint[] spawnPoints = SharedResources.LoadSpawnPoints(activeScene);

        // This player's assigned spawnpoint
        int spawnIndex = PhotonNetwork.LocalPlayer.ActorNumber - 1;

        if (spawnIndex == -1)
            print("Index is -1 | Actornumber not found.");

        LocalSpawnPoint playerStart = spawnPoints[spawnIndex];

        // Player Ship
        PlayerShip playerShip = PhotonNetwork.Instantiate(playerPath, playerStart.transform.position, playerStart.transform.rotation).GetComponent<PlayerShip>();

        // User Interface
        UIManager playerUI = Instantiate(playerUIClass);
        playerUI.playerShip = playerShip;
        playerUI.playerCount = playerStatsList.Count;

        // Analytics
        AnalyticsManager analyticsManager = Instantiate(analyticsManagerClass);
        analyticsManager.playerShip = playerShip;

        // Player Camera
        PlayerCamera playerCamera = Instantiate(playerCameraClass);
        playerCamera.target = playerShip;

        //// Radar Camera
        //RadarCamera radarCamera = Instantiate(radarCameraClass);
        //radarCamera.target = playerShip;

        // Camera references
        PlayerController playerController = playerShip.gameObject.GetComponent<PlayerController>();
        playerController.SetPlayerCamera(playerCamera);
        playerShip.SetPlayerCamera(playerCamera);

        SetPlayerShip(player, playerShip);
        SetPlayerCamera(player, playerCamera);

        return playerShip;
    }

    // Set the ship in player stats
    private void SetPlayerShip(Player player, PlayerShip playerShip)
    {
        int index = playerStatsList.FindIndex(i => i.player == player);

        if (Method.IndexFound(index))
        {
            playerStatsList[index].playerShip = playerShip;
        }
    }

    private void SetPlayerCamera(Player player, PlayerCamera playerCamera)
    {
        int index = playerStatsList.FindIndex(i => i.player == player);

        if (Method.IndexFound(index))
        {
            playerStatsList[index].playerCamera = playerCamera;
        }
    }

    // Get player's color preferences from PlayerPrefsX and store in PlayerStats
    public void SetShipSkin(Player player)
    {
        int index = playerStatsList.FindIndex(i => i.player == player);

        if (Method.IndexFound(index))
        {
            ShipSkin skin = new ShipSkin();

            skin.baseColor = PlayerPrefsX.GetColor("REGULAR_COLOR");
            skin.lightColor = PlayerPrefsX.GetColor("LIGHT_COLOR");
            skin.darkColor = PlayerPrefsX.GetColor("DARK_COLOR");

            if (PlayerPrefsX.GetColor("FORCEFIELD_COLOR") != null)
                skin.forcefieldColor = PlayerPrefsX.GetColor("FORCEFIELD_COLOR");

            playerStatsList[index].shipSkin = skin;
        }
    }

    public ShipSkin GetShipSkin(Player player)
    {
        ShipSkin skin = new ShipSkin();

        int index = playerStatsList.FindIndex(i => i.player == player);

        if (Method.IndexFound(index))
        {
            skin = playerStatsList[index].shipSkin;
        }

        return skin;
    }
}

public class PlayerStats
{
    public readonly Player player;

    // Objects
    public PlayerShip playerShip;
    public PlayerCamera playerCamera;

    // Ship attributes
    public int shipHealth;
    public ShipSkin shipSkin;

    public PlayerStats(Player player, int health)
    {
        this.player = player;
        this.shipHealth = health;
    }
}
