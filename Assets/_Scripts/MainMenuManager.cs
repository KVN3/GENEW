using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : LevelSingleton<MainMenuManager>
{
    [Header("Text")]
    public TextMeshProUGUI TitleText;
    public TextMeshProUGUI PlayText;
    public TextMeshProUGUI OptionsText;
    public TextMeshProUGUI VersionText;

    // Start is called before the first frame update
    void Start()
    {
        TitleText.text = LocalizationManager.GetTextByKey("MAIN_MENU");
        PlayText.text = LocalizationManager.GetTextByKey("PLAY");
        OptionsText.text = LocalizationManager.GetTextByKey("OPTIONS");
        VersionText.text = LocalizationManager.GetTextByKey("VERSION")+": "+ "Vertical Slice";
    }
}
