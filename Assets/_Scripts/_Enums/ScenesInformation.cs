using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum SceneTitle
{
    MAIN, SHIPYARD, WASTELAND, HIGHWAY, TEST, TUTORIAL
}

public class ScenesInformation : MonoBehaviour
{
    public static Dictionary<SceneTitle, string> sceneNames;

    public void Awake()
    {
        sceneNames = new Dictionary<SceneTitle, string>();
        sceneNames.Add(SceneTitle.MAIN, "Main Menu");
        sceneNames.Add(SceneTitle.SHIPYARD, "Shipyard");
        sceneNames.Add(SceneTitle.WASTELAND, "Eraarlonium Wasteland");
        sceneNames.Add(SceneTitle.TEST, "Test");
        sceneNames.Add(SceneTitle.TUTORIAL, "Tutorial");
        sceneNames.Add(SceneTitle.HIGHWAY, "Elto Highway");
    }

    #region SCENE SPECIFIC GAME SETTINGS
    public static int GetDesiredChaserAliveCount(string sceneName)
    {
        int desiredAliveCount = 0;

        switch (sceneName)
        {
            case "Eraarlonium Wasteland":
                desiredAliveCount = 14;
                break;
            case "Elto Highway":
                desiredAliveCount = 15;
                break;
            case "Tutorial":
                desiredAliveCount = 3;
                break;
        }

        return desiredAliveCount;
    }

    public static int GetDesiredShooterAliveCount(string sceneName)
    {
        int desiredAliveCount = 0;

        switch (sceneName)
        {
            case "Eraarlonium Wasteland":
                desiredAliveCount = 4;
                break;
            case "Elto Highway":
                desiredAliveCount = 1;
                break;
            case "Tutorial":
                desiredAliveCount = 3;
                break;
        }

        return desiredAliveCount;
    }

    public static bool IsTutorialScene()
    {
        string sceneName = SceneManager.GetActiveScene().name;

        if (sceneName.Equals("Tutorial"))
            return true;
        else
            return false;
    }

    #endregion


    #region abstract
    protected static ScenesInformation _Instance;

    public static bool Initialized => _Instance != null;

    public static ScenesInformation Instance
    {
        get
        {
            if (!Initialized)
            {
                GameObject GameObject = new GameObject("ScenesInformation");

                _Instance = GameObject.AddComponent<ScenesInformation>();

                _Instance.gameObject.AddComponent<DDOL>();
            }

            return _Instance;
        }
    }

    [RuntimeInitializeOnLoadMethod]
    static void ForceInit()
    {
        ScenesInformation SI = Instance;
    }


    #endregion
}
