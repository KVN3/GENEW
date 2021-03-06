﻿using System.Collections;
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
    public GameObject friendlistBtn;

    [Header("Text")]
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI playText;
    public TextMeshProUGUI shipyardText; 
    public TextMeshProUGUI achievementsText;
    public TextMeshProUGUI logoutText;
    public TextMeshProUGUI exitGameText;
    public TextMeshProUGUI versionText;

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
        // Set sprite states
        if (!ClientConfigurationManager.Instance.clientConfiguration.MusicOn)
            musicIcon.sprite = musicIconOff;
        if (!ClientConfigurationManager.Instance.clientConfiguration.SoundOn)
            soundIcon.sprite = soundIconOff;

        friendlistBtn.SetActive(true);
    }

    void Update()
    {
        // Localization
        titleText.text = LocalizationManager.GetTextByKey("MAIN_MENU");
        playText.text = LocalizationManager.GetTextByKey("PLAY");
        shipyardText.text = LocalizationManager.GetTextByKey("SHIPYARD");
        achievementsText.text = LocalizationManager.GetTextByKey("ACHIEVEMENTS");
        logoutText.text = LocalizationManager.GetTextByKey("LOGOUT");
        exitGameText.text = LocalizationManager.GetTextByKey("EXIT_GAME");
        versionText.text = LocalizationManager.GetTextByKey("VERSION") + ": " + " 1.0";
    }

    #region Toggles
    public void ToggleSound()
    {
        if (soundIcon.sprite == soundIconOff)
        {
            soundIcon.sprite = soundIconOn;
            ClientConfigurationManager.Instance.clientConfiguration.SoundOn = true;
        }
        else
        {
            soundIcon.sprite = soundIconOff;
            ClientConfigurationManager.Instance.clientConfiguration.SoundOn = false;
        }
        ClientConfigurationManager.Instance.SavePlayerSettings();
    }

    public void ToggleMusic()
    {
        if (musicIcon.sprite == musicIconOff)
        {
            musicIcon.sprite = musicIconOn;
            ClientConfigurationManager.Instance.clientConfiguration.MusicOn = true;
        }
        else
        {
            musicIcon.sprite = musicIconOff;
            ClientConfigurationManager.Instance.clientConfiguration.MusicOn = false;
            
        }
        ClientConfigurationManager.Instance.SavePlayerSettings();
    }
    #endregion
}
