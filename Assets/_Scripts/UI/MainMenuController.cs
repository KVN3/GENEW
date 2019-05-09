using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class MainMenuController : MonoBehaviour
{
    public GameObject loadingScreen;

    public Slider loadingBar;
    public TextMeshProUGUI progressText;
    public TextMeshProUGUI loadingText;

    public AchievementCanvas achievementCanvas;

    [SerializeField]
    private MenuSoundManager menuSoundManagerClass;
    

    private MenuSoundManager menuSoundManager;

    void Awake()
    {
        // Only create one instance
        if (GameObject.Find(achievementCanvas.name+"(Clone)") == null)
            achievementCanvas = Instantiate(achievementCanvas);
        menuSoundManager = Instantiate(menuSoundManagerClass, transform.localPosition, transform.localRotation, this.transform);
        
    }

    public void PlayButtonSound()
    {
        menuSoundManager.PlaySound(SoundType.CLICKBUTTON);
    }






    public void UpdateRoomList()
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            bool success = PhotonNetwork.GetCustomRoomList(TypedLobby.Default, "C0 = 0");

            if (!success)
            {
                print("Refreshing rooms through UpdateRoomlist failed. Ignore this for now.");
            }
        }

    }

    public void Disconnect()
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            PhotonNetwork.Disconnect();
        }
    }

    public void Connect()
    {
        LobbyNetwork.Connect();
    }






    public void LoadLevel()
    {
        SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
        if (loadingScreen != null)
            StartCoroutine(LoadAsynchronously(ScenesInformation.sceneNames[SceneTitle.Wasteland]));
        else
            SceneManager.LoadScene(ScenesInformation.sceneNames[SceneTitle.Wasteland]);
    }

    public void LoadShipyard()
    {
        SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
        if (loadingScreen != null)
            StartCoroutine(LoadAsynchronously(ScenesInformation.sceneNames[SceneTitle.Shipyard]));
        else
            SceneManager.LoadScene(ScenesInformation.sceneNames[SceneTitle.Shipyard]);
    }

    public void LoadMainMenu()
    {
        SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
        SceneManager.LoadScene(ScenesInformation.sceneNames[SceneTitle.Main]);
    }

    public void LoadMainMenuFromLevel()
    {
        PlayerNetwork.ReturnToMain();
    }

    #region Coroutines LoadAsync

    IEnumerator LoadAsynchronously(string sceneName)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        loadingScreen.SetActive(true);

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / .9f);

            loadingBar.value = progress;
            progressText.text = (progress * 100).ToString("F0") + "%";

            yield return null;
        }
    }

    #endregion


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
