using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : LevelSingleton<MainMenuManager>
{
    [Header("Text")]
    public TextMeshProUGUI TitleText;
    public TextMeshProUGUI SingleplayerText;
    public TextMeshProUGUI OptionsText;
    public TextMeshProUGUI VersionText;

    // Start is called before the first frame update
    void Update()
    {
        TitleText.text = LocalizationManager.GetTextByKey("MAIN_MENU");
        SingleplayerText.text = LocalizationManager.GetTextByKey("SINGLEPLAYER");
        //OptionsText.text = LocalizationManager.GetTextByKey("OPTIONS");
        VersionText.text = LocalizationManager.GetTextByKey("VERSION")+": "+ "Vertical Slice";
    }
}
