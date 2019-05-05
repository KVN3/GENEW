using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapSelection : MonoBehaviour
{
    [SerializeField]
    private Dropdown _dropdownList;
    [SerializeField]
    private List<SceneTitle> scenes;

    private void Start()
    {
        // Populate dropdownlist
        List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();

        foreach (SceneTitle scene in scenes)
        {
            options.Add(new Dropdown.OptionData(ScenesInformation.sceneNames[scene]));
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
