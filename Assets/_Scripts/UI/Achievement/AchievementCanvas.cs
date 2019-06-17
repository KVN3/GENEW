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
}
