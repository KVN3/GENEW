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

    public GameObject friendPanel;

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

    public Button tutorialButton;
    //private MenuSoundManager menuSoundManager;

    protected override void Awake()
    {
        // menuSoundManager = Instantiate(menuSoundManagerClass, transform.localPosition, transform.localRotation, this.transform);
    }

    private void Start()
    {
        Account account = Registration.GetCurrentAccount();
        if (account == null)
            friendPanel.SetActive(false);
        else
            friendPanel.SetActive(true);

    }

    void OnEnable()
    {
        if (!ClientConfigurationManager.Instance.clientConfiguration.MusicOn)
            musicIcon.sprite = musicIconOff;
        if (!ClientConfigurationManager.Instance.clientConfiguration.SoundOn)
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
        versionText.text = LocalizationManager.GetTextByKey("VERSION") + ": " + " Release Candidate";

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
            ClientConfigurationManager.Instance.clientConfiguration.SoundOn = true;
            ClientConfigurationManager.Instance.SavePlayerSettings();
        }
        else
        {
            soundIcon.sprite = soundIconOff;
            ClientConfigurationManager.Instance.clientConfiguration.SoundOn = false;
            ClientConfigurationManager.Instance.SavePlayerSettings();
        }
    }

    public void ToggleMusic()
    {
        if (musicIcon.sprite == musicIconOff)
        {
            musicIcon.sprite = musicIconOn;
            ClientConfigurationManager.Instance.clientConfiguration.MusicOn = true;
            ClientConfigurationManager.Instance.SavePlayerSettings();
        }
        else
        {
            musicIcon.sprite = musicIconOff;
            ClientConfigurationManager.Instance.clientConfiguration.MusicOn = false;
            ClientConfigurationManager.Instance.SavePlayerSettings();
        }

    }
    #endregion
}
