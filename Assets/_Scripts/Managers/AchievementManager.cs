using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class AchievementManager : LevelSingleton<AchievementManager>
{
    private readonly string key = "AchievementData";

    protected override void Awake()
    {
        base.Awake();

        // Create and save achievements
        if (!PlayerPrefs.HasKey(key))
            InitAchievements();
    }

    public void InitAchievements()
    {
        // Create achivements
        List<Achievement> achievements = new List<Achievement>
        {
            new Achievement(0, "Fast lap 1", "Beat bronze lap time on Eraarlonium Wasteland", AchievementType.LAPTIME, 1f),
            new Achievement(1, "Fast lap 2", "Beat silver lap time on Eraarlonium Wasteland", AchievementType.LAPTIME, 1f),
            new Achievement(2, "Fast lap 3", "Beat gold lap time on Eraarlonium Wasteland", AchievementType.LAPTIME, 1f),
            new Achievement(3, "Fast lap 4", "Beat platinum lap time on Eraarlonium Wasteland", AchievementType.LAPTIME, 1f)
        };

        // Save
        AchievementData achievementData = new AchievementData { achievements = achievements };
        string json = JsonUtility.ToJson(achievementData);
        PlayerPrefs.SetString(key, json);
        PlayerPrefs.Save();
    }
    
    public List<Achievement> CreateAchievementListForPlayer(Account account)
    {
        string jsonString = PlayerPrefs.GetString(key);
        AchievementData achievementData = JsonUtility.FromJson<AchievementData>(jsonString);

        // Add name to achievements
        foreach (Achievement achiev in achievementData.achievements)
            achiev.user = account.username;

        return achievementData.achievements;
    }

    // Needs index of which achievement to update
    public static void UpdateAchievement(int index, float addedProgress)
    {
        // Load
        Account account = Registration.GetCurrentAccount();

        // Get achievement
        Achievement achievement = account.achievements[index];

        // Add progress
        if (achievement.progress < achievement.progressGoal)
            achievement.progress += addedProgress;
        
        // Check progress
        if (achievement.progress >= achievement.progressGoal)
        {
            achievement.progress = achievement.progressGoal;
            achievement.isDone = true;
            achievement.dateCompleted = DateTime.Now;
        }

        // Update in currentAccount && accountDatabase
        account.achievements[index] = achievement;
        Registration.SaveCurrentAccount(account);
        Registration.SaveAccountToAccountData(account);
    }

    private void ResetAchievement(int index)
    {
        // Load
        Account account = Registration.GetCurrentAccount();

        // Get achievement
        Achievement achievement = account.achievements[index];

        achievement.progress = 0f;
        achievement.isDone = false;
        achievement.dateCompleted = new DateTime();

        // Update in currentAccount && accountDatabase
        account.achievements[index] = achievement;
        Registration.SaveCurrentAccount(account);
        Registration.SaveAccountToAccountData(account);
    }

    // Not used
    public List<Achievement> GetAchievementsByName(string name)
    {
        string jsonString = PlayerPrefs.GetString(key);
        AchievementData achievementData = JsonUtility.FromJson<AchievementData>(jsonString);
        return achievementData.achievements.Where(a => a.user == name).ToList();
    }

    [ContextMenu("Complete all achievements")]
    public void CompleteAllAchievements()
    {
        UpdateAchievement(0, 1f);
        UpdateAchievement(1, 1f);
        UpdateAchievement(2, 1f);
        UpdateAchievement(3, 1f);
    }

    [ContextMenu("Reset Achievements")]
    public void ResetAchievements()
    {
        ResetAchievement(0);
        ResetAchievement(1);
        ResetAchievement(2);
        ResetAchievement(3);
    }

    [ContextMenu("Delete AchievementData")]
    public void DeleteAchievementData()
    {
        PlayerPrefs.DeleteKey(key);
    }
}

public class AchievementData
{
    public List<Achievement> achievements;
}
[Serializable]
public class Achievement
{
    public int id;
    public string title;
    public string description;
    public AchievementType type;
    public DateTime dateCompleted;
    public string user;
    public float progress;
    public float progressGoal;
    public bool isDone;

    public Achievement(int id, string title, string description, AchievementType type, float progressGoal)
    {
        this.id = id;
        this.title = title;
        this.description = description;
        this.type = type;
        this.isDone = false;
        this.progress = 0f;
        this.progressGoal = progressGoal;
    }
}

public enum AchievementType
{
    LAPTIME
}