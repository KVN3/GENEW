using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : LevelSingleton<MainMenuManager>
{
   // [SerializeField]
    //private MenuSoundManager menuSoundManagerClass;

    [Header("Text")]
    public TextMeshProUGUI TitleText;
    public TextMeshProUGUI SingleplayerText;
    public TextMeshProUGUI OptionsText;
    public TextMeshProUGUI VersionText;

    //private MenuSoundManager menuSoundManager;

    new void Awake()
    {
       // menuSoundManager = Instantiate(menuSoundManagerClass, transform.localPosition, transform.localRotation, this.transform);
    }

    // Start is called before the first frame update
    void Update()
    {
        TitleText.text = LocalizationManager.GetTextByKey("MAIN_MENU");
        SingleplayerText.text = LocalizationManager.GetTextByKey("SINGLEPLAYER");
        //OptionsText.text = LocalizationManager.GetTextByKey("OPTIONS");
        VersionText.text = LocalizationManager.GetTextByKey("VERSION")+": "+ "Alpha 1";
    }
}
