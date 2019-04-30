using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SceneTitle
{
    Main, Shipyard, Wasteland
}

public class ScenesInformation : MonoBehaviour
{
    public static Dictionary<SceneTitle, string> sceneNames;

    public void Awake()
    {
        sceneNames = new Dictionary<SceneTitle, string>();
        sceneNames.Add(SceneTitle.Main, "Main Menu");
        sceneNames.Add(SceneTitle.Shipyard, "Shipyard");
        sceneNames.Add(SceneTitle.Wasteland, "Eraarlonium Wasteland");
    }

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
