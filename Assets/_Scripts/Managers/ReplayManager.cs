﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

// ReplayData is per stage
// ReplayData contains replays
// 

public class ReplayManager : LevelSingleton<ReplayManager>
{
    private List<Replay> replayList;

    private readonly string key = "ReplayData";

    protected override void Awake()
    {
        base.Awake();

        // Load replay
        if (PlayerPrefs.HasKey(key))
        {
            string jsonString = PlayerPrefs.GetString(key);
            ReplayData replayData = JsonUtility.FromJson<ReplayData>(jsonString);
        }
        else // Create and save new replays
        {
            InitReplay();
        }
    }

    public void InitReplay()
    {
        // Create
        replayList = new List<Replay>();

        // Save
        ReplayData replayData = new ReplayData { replays = replayList };
        string json = JsonUtility.ToJson(replayData);
        PlayerPrefs.SetString(key, json);
    }

    public void SaveReplay(string playerName, string stage, List<float> posx, List<float> posy, List<float> posz, List<Quaternion> rotations, string lapTime)
    {
        // Load
        string jsonString = PlayerPrefs.GetString(key);
        ReplayData replayData = JsonUtility.FromJson<ReplayData>(jsonString);

        // Clear if there are too many replays
        if (replayData.replays.Count > 1)
            replayData.replays.Clear();

        if (posx.Count > 10000)
            return;

        // Create new replayData if null
        if (replayData == null)
        {
            replayList = new List<Replay>();
            replayData = new ReplayData { replays = replayList };
        }

        // Create
        Replay replay = new Replay { id = replayData.replays.Count, playerName = playerName, stage = stage, posx = posx, posy = posy, posz = posz, rotations = rotations, lapTime = lapTime };

        // Add
        replayData.replays.Add(replay);

        // Update
        string json = JsonUtility.ToJson(replayData);
        PlayerPrefs.SetString(key, json);
    }

    #region Get replays
    public List<Replay> GetReplaysByStage(string stage)
    {
        string jsonString = PlayerPrefs.GetString(key);
        ReplayData replayData = JsonUtility.FromJson<ReplayData>(jsonString);
        if (replayData.replays == null)
            return new List<Replay>();
        return replayData.replays.Where(r => r.stage == stage).ToList();
    }

    public List<Replay> GetReplaysByName(string stage, string name)
    {
        string jsonString = PlayerPrefs.GetString(key);
        ReplayData replayData = JsonUtility.FromJson<ReplayData>(jsonString);
        return replayData.replays.Where(r => r.playerName == name).ToList();
    }

    public Replay GetReplayById(string stage, int id)
    {
        string jsonString = PlayerPrefs.GetString(key);
        ReplayData replayData = JsonUtility.FromJson<ReplayData>(jsonString);
        
        return replayData.replays.FirstOrDefault(x => x.id == id);
    }
    #endregion

    [ContextMenu("Delete Replay Data")]
    public void DeleteReplayData()
    {
        PlayerPrefs.DeleteKey(key);
    }
}

public class ReplayData
{
   public List<Replay> replays;
}
[System.Serializable]
public class Replay
{
    public int id;
    public string playerName;
    public string stage;
    public List<Quaternion> rotations;
    public List<float> posx;
    public List<float> posy;
    public List<float> posz;
    public string lapTime;
}