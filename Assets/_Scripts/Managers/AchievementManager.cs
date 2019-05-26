using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class AchievementManager : MonoBehaviour
{
    private readonly string key = "AchievementData";

    public AchievementCanvas achievementCanvas;

    void Awake()
    {
        DontDestroyOnLoad(this);

        // Create and save achievements
        if (!PlayerPrefs.HasKey(key))
            InitAchievements();

        achievementCanvas = GameObject.FindObjectOfType<AchievementCanvas>();
    }

    public void InitAchievements()
    {
        // Create achivements
        List<Achievement> achievements = new List<Achievement>
        {
            new Achievement(0, "Bronze wasteland", "Beat bronze time on Eraarlonium Wasteland", AchievementType.LAPTIME, 1f),
            new Achievement(1, "Silver wasteland", "Beat silver time on Eraarlonium Wasteland", AchievementType.LAPTIME, 1f),
            new Achievement(2, "Gold wasteland", "Beat gold time on Eraarlonium Wasteland", AchievementType.LAPTIME, 1f),
            new Achievement(3, "Bronze highway", "Beat bronze time on Elto Highway", AchievementType.LAPTIME, 1f),
            new Achievement(4, "Silver highway", "Beat silver time on Elto Highway", AchievementType.LAPTIME, 1f),
            new Achievement(5, "Gold highway", "Beat gold time on Elto Highway", AchievementType.LAPTIME, 1f),
        };

        // Save
        AchievementData achievementData = new AchievementData { achievements = achievements };
        string json = JsonUtility.ToJson(achievementData);
        PlayerPrefs.SetString(key, json);
    }
    
    public List<Achievement> CreateAchievementListForPlayer(Account account)
    {
        List<Achievement> achievements = LoadAchievements();

        // Add user to achievements
        foreach (Achievement achiev in achievements)
            achiev.user = account.username;

        return achievements;
    }

    // Needs index of which achievement to update
    public static void UpdateAchievement(int index, float addedProgress)
    {
        // Load
        Account account = Registration.GetCurrentAccount();

        // Get achievement
        Achievement achievement = account.achievements[index];
        if (!achievement.isDone)
        {
            // Add progress
            if (achievement.progress < achievement.progressGoal)
                achievement.progress += addedProgress;

            // Check progress
            if (achievement.progress >= achievement.progressGoal)
            {
                achievement.progress = achievement.progressGoal;
                achievement.isDone = true;
                achievement.dateCompleted = DateTime.Now;
                Instance.achievementCanvas.InstantiateNotif(achievement);
            }

            // Update in currentAccount && accountDatabase
            account.achievements[index] = achievement;
            Registration.SaveAccount(account);
        }
    }

    private List<Achievement> LoadAchievements()
    {
        string jsonString = PlayerPrefs.GetString(key);
        AchievementData achievementData = JsonUtility.FromJson<AchievementData>(jsonString);
        return achievementData.achievements;
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
        Registration.SaveAccount(account);
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
        UpdateAchievement(4, 1f);
        UpdateAchievement(5, 1f);
    }

    [ContextMenu("Reset Personal Achievements")]
    public void ResetAchievements()
    {
        for (int i = 0; i < LoadAchievements().Count; i++)
        {
            ResetAchievement(i);
        }

    }

    [ContextMenu("Delete AchievementData")]
    public void DeleteAchievementData()
    {
        PlayerPrefs.DeleteKey(key);
        InitAchievements();
        Account account = Registration.GetCurrentAccount();
        account.achievements = CreateAchievementListForPlayer(account);
        Registration.SaveAccount(account);
    }

    #region Singleton

    // Abstract

    protected static AchievementManager _Instance;

    public static bool Initialized => _Instance != null;

    public static AchievementManager Instance
    {
        get
        {
            if (!Initialized)
            {
                GameObject gameObject = new GameObject("Achievement Manager");

                _Instance = gameObject.AddComponent<AchievementManager>();
            }

            return _Instance;
        }
    }

    [RuntimeInitializeOnLoadMethod]
    static void ForceInit()
    {
        AchievementManager GI = Instance;
    }
    #endregion
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
    public float progress;
    public float progressGoal;
    public bool isDone;

    public string user;

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