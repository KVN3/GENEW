using System;
using UnityEngine;

public class ClientConfigurationManager : MonoBehaviour
{
    #region Fields

    private string key = "PlayerConfiguration";

    public ClientConfiguration clientConfiguration { get; set; }

    #endregion

    private void Awake()
    {
        if (PlayerPrefs.HasKey(key))
            LoadPlayerSettings();
        else
            InitDefaultSettings();
    }

    private void InitDefaultSettings()
    {
        if (clientConfiguration == null)
            clientConfiguration = new ClientConfiguration();

        clientConfiguration.IsFirstTimePlaying = true;
        clientConfiguration.SawChangelog = false;
        clientConfiguration.SawNewAchievement = true;

        clientConfiguration.MusicOn = true;
        clientConfiguration.SoundOn = true;

        clientConfiguration.SoundVolume = 0;
        clientConfiguration.MusicVolume = 0;

        SavePlayerSettings();
    }

    #region Save/Load
    public void SavePlayerSettings()
    {
        string json = JsonUtility.ToJson(clientConfiguration);
        PlayerPrefs.SetString(key, json);
    }

    private void LoadPlayerSettings()
    {
        string jsonString = PlayerPrefs.GetString(key);
        clientConfiguration = JsonUtility.FromJson<ClientConfiguration>(jsonString);
    }
    #endregion

    [ContextMenu("Reset All Settings")]
    public void ResetAllSettings()
    {
        PlayerPrefs.DeleteKey(key);
    }

    #region Singleton

    // Abstract

    protected static ClientConfigurationManager _Instance;

    public static bool Initialized => _Instance != null;

    public static ClientConfigurationManager Instance
    {
        get
        {
            if (!Initialized)
            {
                GameObject gameObject = new GameObject("Client Configuration Manager");

                _Instance = gameObject.AddComponent<ClientConfigurationManager>();
            }

            return _Instance;
        }
    }

    [RuntimeInitializeOnLoadMethod]
    static void ForceInit()
    {
        ClientConfigurationManager GI = Instance;
    }
    #endregion
}