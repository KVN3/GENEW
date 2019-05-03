using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class SharedResources
{
    #region Player
    public static AnalyticsManager LoadAnalyticsManager()
    {
        string analyticsManagerPath = Path.Combine("Prefabs", "Player", "AnalyticsManager");
        GameObject analyticsManagerObj = Resources.Load(analyticsManagerPath) as GameObject;

        return analyticsManagerObj.GetComponent<AnalyticsManager>();
    }

    public static UIManager LoadUI()
    {
        string userInterfacePath = Path.Combine("Prefabs", "Player", "UI", "PlayerUI");
        GameObject UIObj = Resources.Load(userInterfacePath) as GameObject;
        UIManager UIManager = UIObj.GetComponent<UIManager>();
        UIManager.HUDClass = LoadHUD();

        return UIManager;
    }

    public static HUD LoadHUD()
    {
        string userInterfacePath = Path.Combine("Prefabs", "Player", "UI", "HUD");
        GameObject HUDObj = Resources.Load(userInterfacePath) as GameObject;

        return HUDObj.GetComponent<HUD>();
    }

    public static PlayerShip LoadPlayerShip()
    {
        string playerPath = Path.Combine("Prefabs", "Player", "PlayerShip");
        GameObject playerShipObj = Resources.Load(playerPath) as GameObject;

        return playerShipObj.GetComponent<PlayerShip>();
    }

    public static PlayerCamera LoadPlayerCamera()
    {
        string cameraPath = Path.Combine("Prefabs", "Player", "PlayerCamera");
        GameObject playerCameraObj = Resources.Load(cameraPath) as GameObject;

        return playerCameraObj.GetComponent<PlayerCamera>();
    }
    #endregion

    #region Enemies
    //public static Chaser[] LoadChasers(string activeScene)
    //{
    //    string spawnpointsPath = Path.Combine("Prefabs", "Spawnpoints", activeScene);

    //    Object[] objects = Resources.LoadAll(spawnpointsPath);
    //    LocalSpawnPoint[] spawnPoints = new LocalSpawnPoint[objects.Length];

    //    int i = 0;
    //    foreach (Object obj in objects)
    //    {
    //        GameObject gameObject = obj as GameObject;
    //        spawnPoints[i] = gameObject.GetComponent<LocalSpawnPoint>();
    //        i++;
    //    }

    //    return spawnPoints;
    //}
    #endregion

    #region Spawnpoints
    public static LocalSpawnPoint[] LoadSpawnPoints(string sceneName)
    {
        string spawnpointsPath = GetPath("SpawnPoints", sceneName);

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

    #region Paths
    public static string GetPath(string prefabName)
    {
        string path = string.Empty;
        string currentScene = ScenesInformation.sceneNames[SceneTitle.Wasteland];

        switch (prefabName)
        {
            case "ShooterModule":
                path = Path.Combine("Prefabs", "Enemies", "Modules", "Shooter", "ShooterModule");
                break;
            case "EnergyBallProjectile":
                path = Path.Combine("Prefabs", "Enemies", "Modules", "Shooter", "EnergyBallProjectile");
                break;
            case "CirclingEnergyBall":
                path = Path.Combine("Prefabs", "Enemies", "Circlers", "CirclingEnergyBall");
                break;
        }

        return path;
    }

    public static string GetPath(string prefabName, SceneTitle sceneTitle)
    {
        string path = string.Empty;
        string currentScene = ScenesInformation.sceneNames[sceneTitle];

        switch (prefabName)
        {
            case "OrbitPoint":
                path = Path.Combine("Prefabs", "SpawnPoints", currentScene, "Orbits", prefabName);
                break;
        }

        return path;
    }

    public static string GetPath(string prefabName, string sceneName)
    {
        string path = string.Empty;

        switch (prefabName)
        {
            case "OrbitPoint":
                path = Path.Combine("Prefabs", "SpawnPoints", sceneName, "Orbits", prefabName);
                break;
            case "SpawnPoints":
                path = Path.Combine("Prefabs", "SpawnPoints", sceneName, "PlayerSpawns");
                break;
        }

        return path;
    }
    #endregion




}
