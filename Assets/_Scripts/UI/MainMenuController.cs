﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : LevelSingleton<MainMenuController>
{
    [SerializeField]
    private MenuSoundManager menuSoundManagerClass;

    private MenuSoundManager menuSoundManager;

    new void Awake()
    {
        menuSoundManager = Instantiate(menuSoundManagerClass, transform.localPosition, transform.localRotation, this.transform);
    }

    public void PlayButtonSound()
    {
        menuSoundManager.PlaySound(SoundType.CLICKBUTTON);
    }

    public void LoadLevel()
    {
        SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
        SceneManager.LoadScene(ScenesInformation.sceneNames[SceneTitle.Wasteland]);
    }

    public void LoadShipyard()
    {
        SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
        SceneManager.LoadScene(ScenesInformation.sceneNames[SceneTitle.Shipyard]);
    }

    public void LoadMainMenu()
    {
        SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
        SceneManager.LoadScene(ScenesInformation.sceneNames[SceneTitle.Main]);
    }

    public void SetColorBlue()
    {
        PlayerPrefs.SetString("Ship Color", "Blue");
    }
    public void SetColorRed()
    {
        PlayerPrefs.SetString("Ship Color", "Red");
    }
    public void SetColorGreen()
    {
        PlayerPrefs.SetString("Ship Color", "Green");
    }
}
