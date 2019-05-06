using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HighscoreManager : LevelSingleton<HighscoreManager>
{
    private List<HighscoreEntry> highscoreEntryList;

    private readonly string key = "HighscoreTable";
    private readonly string personalKey = "Highscore";

    protected override void Awake()
    {
        base.Awake();
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
            new HighscoreEntry { name = "Bronze", lapTime = "00:00:50.000", stage = ScenesInformation.sceneNames[SceneTitle.Wasteland]},
            new HighscoreEntry { name = "Silver", lapTime = "00:00:45.000", stage = ScenesInformation.sceneNames[SceneTitle.Wasteland]},
            new HighscoreEntry { name = "Gold", lapTime = "00:00:40.000", stage = ScenesInformation.sceneNames[SceneTitle.Wasteland]},
            new HighscoreEntry { name = "Platinum", lapTime = "00:00:35.000", stage = ScenesInformation.sceneNames[SceneTitle.Wasteland]}
        };

        // Sort
        SortHighscores(highscoreEntryList);

        // Save
        Highscores highscores = new Highscores { highscoreEntryList = highscoreEntryList };
        string json = JsonUtility.ToJson(highscores);
        PlayerPrefs.SetString(key, json);
        PlayerPrefs.Save();
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
        PlayerPrefs.Save();
    }

    //public List<HighscoreEntry> GetHighscores()
    //{
    //    string jsonString = PlayerPrefs.GetString(key);
    //    Highscores highscores = JsonUtility.FromJson<Highscores>(jsonString);
    //    return highscores.highscoreEntryList;
    //}

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

    public void ResetHighscores()
    {
        PlayerPrefs.DeleteKey(key);
        PlayerPrefs.DeleteKey(personalKey);
        InitHighscores();
    }
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