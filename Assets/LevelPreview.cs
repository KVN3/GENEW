using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelPreview : MonoBehaviour
{
    public MapSelection mapSelection;
    public GameObject[] levelPreviewImages;

    public void UpdateLevelPreview()
    {
        SceneTitle sceneTitle = mapSelection.GetScene();
        string sceneName = ScenesInformation.sceneNames[sceneTitle];

        foreach (GameObject gameObject in levelPreviewImages)
        {
            if (gameObject.name.Equals(sceneName))
            {
                gameObject.SetActive(true);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
    }
}
