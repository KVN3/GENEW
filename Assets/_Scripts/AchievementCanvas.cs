using System.Collections;
using TMPro;
using UnityEngine;

public class AchievementCanvas : MonoBehaviour
{
    #region Fields
    public static AchievementCanvas instance;

    public GameObject panel;
    public GameObject achievementNotifPrefab;
    #endregion

    #region Unity Methods
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
        { 
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        DontDestroyOnLoad(this);
    }

    public void InstantiateNotif(Achievement achievement)
    {
        StartCoroutine(Notif(achievement));
    }

    IEnumerator Notif(Achievement achievement)
    {
        GameObject obj = Instantiate(achievementNotifPrefab) as GameObject;
        obj.SetActive(true);
        obj.GetComponent<AchievementNotification>().SetText(achievement);

        obj.transform.SetParent(achievementNotifPrefab.transform.parent);
        yield return new WaitForSeconds(5f);

        Destroy(obj);
    }
    #endregion

    //#region Singleton

    //// Abstract

    //protected static AchievementCanvas _Instance;

    //public static bool Initialized => _Instance != null;

    //public static AchievementCanvas Instance
    //{
    //    get
    //    {
    //        if (!Initialized)
    //        {
    //            GameObject gameObject = new GameObject("Achievement Canvas");

    //            GameObject panel = new GameObject();

    //            // Set parents
    //            panel.transform.parent = gameObject.transform;

    //            _Instance = gameObject.AddComponent<AchievementCanvas>();
    //        }

    //        return _Instance;
    //    }
    //}

    //[RuntimeInitializeOnLoadMethod]
    //static void ForceInit()
    //{
    //    AchievementCanvas GI = Instance;
    //}
    //#endregion
}
