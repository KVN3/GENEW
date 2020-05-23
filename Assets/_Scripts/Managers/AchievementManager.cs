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

        InitAchievements();

        achievementCanvas = GameObject.FindObjectOfType<AchievementCanvas>();

        CheckNewAchievements();
        //GetNewAchievements();
    }

    public void InitAchievements()
    {
        List<Achievement> achievements = new List<Achievement>
        {
            new Achievement(0, LocalizationManager.GetTextByKey("A1_NAME"), LocalizationManager.GetTextByKey("A1_DESC"), AchievementType.EVENT, 1f), // DONE
            new Achievement(1, LocalizationManager.GetTextByKey("A2_NAME"), LocalizationManager.GetTextByKey("A2_DESC"), AchievementType.LAPTIME, 1f),
            new Achievement(2, LocalizationManager.GetTextByKey("A3_NAME"), LocalizationManager.GetTextByKey("A3_DESC"), AchievementType.LAPTIME, 1f),
            new Achievement(3, LocalizationManager.GetTextByKey("A4_NAME"), LocalizationManager.GetTextByKey("A4_DESC"), AchievementType.LAPTIME, 1f),
            new Achievement(4, LocalizationManager.GetTextByKey("A5_NAME"), LocalizationManager.GetTextByKey("A5_DESC"), AchievementType.LAPTIME, 1f),
            new Achievement(5, LocalizationManager.GetTextByKey("A6_NAME"), LocalizationManager.GetTextByKey("A6_DESC"), AchievementType.LAPTIME, 1f),
            new Achievement(6, LocalizationManager.GetTextByKey("A7_NAME"), LocalizationManager.GetTextByKey("A7_DESC"), AchievementType.LAPTIME, 1f),
            new Achievement(7, LocalizationManager.GetTextByKey("A8_NAME"), LocalizationManager.GetTextByKey("A8_DESC"), AchievementType.MISC, 1f), // DONE
            new Achievement(8, LocalizationManager.GetTextByKey("A9_NAME"), LocalizationManager.GetTextByKey("A9_DESC"), AchievementType.MISC, 1f), // DONE
            new Achievement(9, LocalizationManager.GetTextByKey("A10_NAME"), LocalizationManager.GetTextByKey("A10_DESC"), AchievementType.MISC, 1f), // DONE
            new Achievement(10, LocalizationManager.GetTextByKey("A11_NAME"), LocalizationManager.GetTextByKey("A11_DESC"), AchievementType.MISC, 1f), // DONE
            new Achievement(11, LocalizationManager.GetTextByKey("A12_NAME"), LocalizationManager.GetTextByKey("A12_DESC"), AchievementType.STAT, 1f), // DONE
            new Achievement(12, LocalizationManager.GetTextByKey("A13_NAME"), LocalizationManager.GetTextByKey("A13_DESC"), AchievementType.STAT, 50f), // DONE
            new Achievement(13, LocalizationManager.GetTextByKey("A14_NAME"), LocalizationManager.GetTextByKey("A14_DESC"), AchievementType.MISC, 1f), // DONE
            new Achievement(14, LocalizationManager.GetTextByKey("A15_NAME"), LocalizationManager.GetTextByKey("A15_DESC"), AchievementType.STAT, 20f), // DONE
            new Achievement(15, LocalizationManager.GetTextByKey("A16_NAME"), LocalizationManager.GetTextByKey("A16_DESC"), AchievementType.MISC, 1f),
            new Achievement(16, LocalizationManager.GetTextByKey("A17_NAME"), LocalizationManager.GetTextByKey("A17_DESC"), AchievementType.STAT, 1f), // DONE
            new Achievement(17, LocalizationManager.GetTextByKey("A18_NAME"), LocalizationManager.GetTextByKey("A18_DESC"), AchievementType.STAT, 1f), // DONE
            new Achievement(18, LocalizationManager.GetTextByKey("A19_NAME"), LocalizationManager.GetTextByKey("A19_DESC"), AchievementType.STAT, 20f), // DONE 
            new Achievement(19, LocalizationManager.GetTextByKey("A20_NAME"), LocalizationManager.GetTextByKey("A20_DESC"), AchievementType.STAT, 50f) // DONE
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

    public static void UpdateAchievementByName(string name, float addedProgress)
    {
        // Load
        Account account = Registration.GetCurrentAccount();

        // Get index
        Achievement achievement = account.achievements.FirstOrDefault(a => a.title == name);
        if (achievement != null)
        { 
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
                account.achievements[achievement.id] = achievement;
                Registration.SaveAccount(account);
            }
        }
        else
            print("Achievement has invalid name");
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

    private void CheckNewAchievements()
    {
        Account account = Registration.GetCurrentAccount();
        if (account != null)
        {
            if (account.achievements.Count < LoadAchievements().Count)
            {
                DeleteAchievementData();
                InitAchievements();
                CreateAchievementListForPlayer(account);
                print("Achievements updated!");
            }
        }
    }

    private void GetNewAchievements()
    {
        Account account = Registration.GetCurrentAccount();
        if (account != null)
        {
            DeleteAchievementData();
            InitAchievements();
            CreateAchievementListForPlayer(account);
        }
    }

    #region Not used
    // Not used
    public List<Achievement> GetAchievementsByName(string name)
    {
        string jsonString = PlayerPrefs.GetString(key);
        AchievementData achievementData = JsonUtility.FromJson<AchievementData>(jsonString);
        return achievementData.achievements.Where(a => a.user == name).ToList();
    }
    #endregion

    [ContextMenu("Complete all achievements")]
    public void CompleteAllAchievements()
    {
        for (int i = 0; i < LoadAchievements().Count; i++)
            UpdateAchievement(i, 500f);
    }

    [ContextMenu("Reset Personal Achievements")]
    public void ResetAchievements()
    {
        for (int i = 0; i < LoadAchievements().Count; i++)
            ResetAchievement(i);
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
    LAPTIME, EVENT, MISC, STAT
}