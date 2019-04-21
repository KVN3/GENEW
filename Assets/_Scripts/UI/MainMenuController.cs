using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : LevelSingleton<MainMenuController>
{
    public void GoToVSLevel()
    {
        SceneManager.LoadScene("Gianni LP");
    }

    public void GoToLevel1()
    {
        SceneManager.LoadScene("Level1");
    }
    public void GoToLevel2()
    {
        SceneManager.LoadScene("Gianni's level");
    }
}
