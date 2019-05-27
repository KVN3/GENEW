using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;

public class HighscoreManager : MonoBehaviour
{
    public static HighscoreManager Instance { get; set; }

    private List<HighscoreEntry> highscoreEntryList;

    private readonly string key = "HighscoreTable";
    private readonly string personalKey = "Highscore";

    private List<GameObject> leaderboardEntries = new List<GameObject>();

    public Sprite rankOne;
    public Sprite rankTwo;
    public Sprite rankThree;
    public Sprite rankOther;

    void Awake()
    {
        DontDestroyOnLoad(this);

        if (Instance == null)
            Instance = this;
        else
            Destroy(this.gameObject);


        // Load highscores
        if (PlayerPrefs.HasKey(key))
        {
            string jsonString = PlayerPrefs.GetString(key);
            Highscores hs = JsonUtility.FromJson<Highscores>(jsonString);
        }
        else // Create and save new highscores
        {
            InitHighscores();
        }
    }

    private void InitHighscores()
    {
        // Create
        highscoreEntryList = new List<HighscoreEntry>()
        {
            new HighscoreEntry { name = "Bronze", lapTime = "00:02:15.000", stage = ScenesInformation.sceneNames[SceneTitle.WASTELAND]},
            new HighscoreEntry { name = "Silver", lapTime = "00:02:05.000", stage = ScenesInformation.sceneNames[SceneTitle.WASTELAND]},
            new HighscoreEntry { name = "Gold", lapTime = "00:02:00.000", stage = ScenesInformation.sceneNames[SceneTitle.WASTELAND]},
            new HighscoreEntry { name = "Bronze", lapTime = "00:02:00.000", stage = ScenesInformation.sceneNames[SceneTitle.HIGHWAY]},
            new HighscoreEntry { name = "Silver", lapTime = "00:01:50.000", stage = ScenesInformation.sceneNames[SceneTitle.HIGHWAY]},
            new HighscoreEntry { name = "Gold", lapTime = "00:01:45.000", stage = ScenesInformation.sceneNames[SceneTitle.HIGHWAY]}
        };

        // Sort
        SortHighscores(highscoreEntryList);

        // Save
        Highscores highscores = new Highscores { highscoreEntryList = highscoreEntryList };
        string json = JsonUtility.ToJson(highscores);
        PlayerPrefs.SetString(key, json);
    }

    public void AddHighscoreEntry(string name, string time, string stage)
    {
        // Create
        HighscoreEntry highscoreEntry = new HighscoreEntry { name = name, lapTime = time, stage = stage };

        // Load
        string jsonString = PlayerPrefs.GetString(key);
        Highscores highscores = JsonUtility.FromJson<Highscores>(jsonString);

        // Add
        highscores.highscoreEntryList.Add(highscoreEntry);

        SortHighscores(highscores.highscoreEntryList);

        // Update
        string json = JsonUtility.ToJson(highscores);
        PlayerPrefs.SetString(key, json);
    }

    public List<HighscoreEntry> GetHighscoresByStage(string stage)
    {
        string jsonString = PlayerPrefs.GetString(key);
        Highscores highscores = JsonUtility.FromJson<Highscores>(jsonString);
        List<HighscoreEntry> list = highscores.highscoreEntryList.Where(h => h.stage == stage).ToList();
        return list;
    }

    public void SortHighscores(List<HighscoreEntry> highscoreEntryList)
    {
        // Sort by time
        for (int i = 0; i < highscoreEntryList.Count; i++)
        {
            for (int j = 0; j < highscoreEntryList.Count; j++)
            {
                if (TimeSpan.Parse(highscoreEntryList[j].lapTime) > TimeSpan.Parse(highscoreEntryList[i].lapTime))
                {
                    // Swap
                    HighscoreEntry tmp = highscoreEntryList[i];
                    highscoreEntryList[i] = highscoreEntryList[j];
                    highscoreEntryList[j] = tmp;
                }
            }
        }
    }

    public void ShowHighscores(string levelTitle, GameObject entryTemplate, GameObject parent)
    {
        // Get highscores (and sorts them beforehand)
        List<HighscoreEntry> highscoreEntries = Instance.GetHighscoresByStage(levelTitle);

        // Only show max of 10 or below
        int entries;
        if (highscoreEntries.Count > 10)
            entries = 10;
        else
            entries = highscoreEntries.Count;

        foreach (GameObject obj in leaderboardEntries)
            Destroy(obj);
        leaderboardEntries.Clear();

        for (int i = 0; i < entries; i++)
        {
            GameObject entry = Instantiate(entryTemplate) as GameObject;
            leaderboardEntries.Add(entry);
            entry.SetActive(true);

            string rankText;
            // Image
            Sprite sprite;
            switch (i+1)
            {
                case 1: sprite = rankOne;
                    rankText = "";
                    break;
                case 2: sprite = rankTwo;
                    rankText = "";
                    break;
                case 3: sprite = rankThree;
                    rankText = "";
                    break;
                default: sprite = rankOther;
                    rankText = (i + 1).ToString();
                    break;
            }

            entry.GetComponent<LeaderboardEntry>().SetUIValues(sprite, rankText, highscoreEntries[i]);

            entry.transform.SetParent(parent.transform, false);
        }
    }

    [ContextMenu("Reset highscores")]
    public void ResetHighscores()
    {
        PlayerPrefs.DeleteKey(key);
        PlayerPrefs.DeleteKey(personalKey);
        InitHighscores();
    }

    #region Singleton

    //// Abstract

    //protected static HighscoreManager _Instance;

    //public static bool Initialized => _Instance != null;

    //public static HighscoreManager Instance
    //{
    //    get
    //    {
    //        if (!Initialized)
    //        {
    //            GameObject gameObject = new GameObject("Highscore Manager");

    //            _Instance = gameObject.AddComponent<HighscoreManager>();
    //        }

    //        return _Instance;
    //    }
    //}

    //[RuntimeInitializeOnLoadMethod]
    //static void ForceInit()
    //{
    //    HighscoreManager GI = Instance;
    //}
    #endregion
}


//  You can't save a list by itself, has to be in a class so you can convert to json
public class Highscores
{
    public List<HighscoreEntry> highscoreEntryList;
}

[System.Serializable]
public class HighscoreEntry
{
    public string name;
    public string lapTime;
    public string stage;
}