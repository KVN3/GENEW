using UnityEngine;
using System.Collections.Generic;

public class AchievementListControl : MonoBehaviour
{
    #region Fields
    public GameObject achievementTemplate;

    private List<GameObject> achievObjects;
    #endregion

    private void Start()
    {
       
    }

    #region Unity Methods
    void OnEnable()
    {
        if (achievObjects == null)
            achievObjects = new List<GameObject>();

        if (achievObjects.Count > 0)
        {
            foreach (GameObject obj in achievObjects)
                Destroy(obj.gameObject);

            achievObjects.Clear();
        }

        Account account = Registration.GetCurrentAccount();
        foreach (Achievement achiev in account.achievements)
        {
            GameObject achievement = Instantiate(achievementTemplate) as GameObject;
            achievObjects.Add(achievement);
            achievement.SetActive(true);

            achievement.GetComponent<AchievementItem>().SetText(achiev.title, achiev.description, achiev.isDone, achiev.progress, achiev.progressGoal);

            achievement.transform.SetParent(achievementTemplate.transform.parent, false);
        }
    }
    #endregion
}
