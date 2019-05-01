using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : LevelSingleton<MainMenuManager>
{
    // [SerializeField]
    //private MenuSoundManager menuSoundManagerClass;

    [Header("Text")]
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI singleplayerText;
    public TextMeshProUGUI editShipText;
    public TextMeshProUGUI versionText;
    public TextMeshProUGUI usernameText;

    //private MenuSoundManager menuSoundManager;

    protected override void Awake()
    {
        // menuSoundManager = Instantiate(menuSoundManagerClass, transform.localPosition, transform.localRotation, this.transform);
    }

    // Start is called before the first frame update
    void Update()
    {
        titleText.text = LocalizationManager.GetTextByKey("MAIN_MENU");
        singleplayerText.text = LocalizationManager.GetTextByKey("SINGLEPLAYER");
        editShipText.text = LocalizationManager.GetTextByKey("EDIT_SHIP");
        versionText.text = LocalizationManager.GetTextByKey("VERSION") + ": " + "Alpha 2";
        if (PlayerPrefs.HasKey("currentAccount"))
            usernameText.text = LocalizationManager.GetTextByKey("LOGGED_IN_AS") + ": " + PlayerPrefs.GetString("currentAccount");
        else
            usernameText.text = LocalizationManager.GetTextByKey("NOT_LOGGED_IN");
    }
}
