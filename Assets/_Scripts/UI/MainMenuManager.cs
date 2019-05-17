using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuManager : LevelSingleton<MainMenuManager>
{
    // [SerializeField]
    //private MenuSoundManager menuSoundManagerClass;

    [Header("Text")]
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI playText;
    public TextMeshProUGUI shipyardText;
    public TextMeshProUGUI optionsText; // Options has sound/music/resetdata/changeAccount
    public TextMeshProUGUI achievementsText;
    public TextMeshProUGUI versionText;
    public TextMeshProUGUI usernameText;

    [Header("Music/Sounds")]
    public Sprite soundIconOn;
    public Sprite soundIconOff;
    public Image soundIcon;

    public Sprite musicIconOn;
    public Sprite musicIconOff;
    public Image musicIcon;

    //private MenuSoundManager menuSoundManager;

    protected override void Awake()
    {
        // menuSoundManager = Instantiate(menuSoundManagerClass, transform.localPosition, transform.localRotation, this.transform);
    }

    void OnEnable()
    {
        if (!GameConfiguration.MusicOn)
            musicIcon.sprite = musicIconOff;
        if (!GameConfiguration.SoundOn)
            soundIcon.sprite = soundIconOff;
    }

    // Start is called before the first frame update
    void Update()
    {
        titleText.text = LocalizationManager.GetTextByKey("MAIN_MENU");
        playText.text = LocalizationManager.GetTextByKey("PLAY");
        shipyardText.text = LocalizationManager.GetTextByKey("SHIPYARD");
        if (achievementsText != null)
            achievementsText.text = LocalizationManager.GetTextByKey("ACHIEVEMENTS");
        versionText.text = LocalizationManager.GetTextByKey("VERSION") + ": " + "Alpha 2";

        if (PlayerPrefs.HasKey("currentAccount"))
        {
            // Load current account
            Account account = Registration.GetCurrentAccount();

            usernameText.text = LocalizationManager.GetTextByKey("LOGGED_IN_AS") + ": " + account.username;
        }
        else
            usernameText.text = LocalizationManager.GetTextByKey("NOT_LOGGED_IN");
    }

    #region Toggles
    public void ToggleSound()
    {
        if (soundIcon.sprite == soundIconOff)
        {
            soundIcon.sprite = soundIconOn;
            GameConfiguration.SoundOn = true;
        }
        else
        {
            soundIcon.sprite = soundIconOff;
            GameConfiguration.SoundOn = false;
        }
    }

    public void ToggleMusic()
    {
        if (musicIcon.sprite == musicIconOff)
        {
            musicIcon.sprite = musicIconOn;
            GameConfiguration.MusicOn = true;
        }
        else
        {
            musicIcon.sprite = musicIconOff;
            GameConfiguration.MusicOn = false;
        }

    }
    #endregion
}
