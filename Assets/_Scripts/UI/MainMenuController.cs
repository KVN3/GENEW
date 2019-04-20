using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public void GoToVSLevel()
    {
        SceneManager.LoadScene("LP");
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
