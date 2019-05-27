using UnityEngine;

public class LeaderboardLayoutGroup : MonoBehaviour
{
    #region Fields
    public MapSelection _mapSelection;

    // Templates/prefabs
    public GameObject entryTemplate;
    #endregion

    #region Unity Methods
    public void UpdateLeaderboard()
    {
        SceneTitle _sceneTitle = _mapSelection.GetScene();
        HighscoreManager.Instance.ShowHighscores(ScenesInformation.sceneNames[_sceneTitle], entryTemplate, this.gameObject);
    }
    #endregion
}
