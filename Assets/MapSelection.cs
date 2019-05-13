using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MapSelection : MonoBehaviour
{
    [SerializeField]
    private TMP_Dropdown _dropdownList;
    [SerializeField]
    private List<SceneTitle> scenes;

    private void Start()
    {
        // Populate dropdownlist
        List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();

        foreach (SceneTitle scene in scenes)
        {
            options.Add(new TMP_Dropdown.OptionData(ScenesInformation.sceneNames[scene]));
        }
        
        _dropdownList.AddOptions(options);
    }

    // Get the selected scene from the dropdownlist
    public SceneTitle GetScene()
    {
        SceneTitle scene = scenes[_dropdownList.value];
        return scene;
    }
}
