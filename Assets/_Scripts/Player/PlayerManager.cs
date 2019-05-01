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
        PlayerCamera playerCameraClass = LoadPlayerCamera();
        UIManager playerUIClass = LoadUI();
        AnalyticsManager analyticsManagerClass = LoadAnalyticsManager();
        LocalSpawnPoint[] spawnPoints = LoadSpawnPoints(activeScene);

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

    //public ShipSkin GetShipSkin(Player player)
    //{
    //    int index = playerStatsList.FindIndex(i => i.player == player);

    //    if (Method.IndexFound(index))
    //    {

    //    }
    //}

    // These methods load files from the Resources folder
    #region Resources
    private AnalyticsManager LoadAnalyticsManager()
    {
        string analyticsManagerPath = Path.Combine("Prefabs", "Player", "AnalyticsManager");
        GameObject analyticsManagerObj = Resources.Load(analyticsManagerPath) as GameObject;

        return analyticsManagerObj.GetComponent<AnalyticsManager>();
    }

    private UIManager LoadUI()
    {
        string userInterfacePath = Path.Combine("Prefabs", "Player", "PlayerUI");
        GameObject UIObj = Resources.Load(userInterfacePath) as GameObject;

        return UIObj.GetComponent<UIManager>();
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

    private LocalSpawnPoint[] LoadSpawnPoints(string activeScene)
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
    #endregion
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
