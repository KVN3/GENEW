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
    #region Fields
    public AchievementCanvas achievementCanvas;

    // Panel references
    public GameObject loadingScreen;
    public GameObject leaderboardPanel;
    public GameObject changelogPanel;

    public Slider loadingBar;
    public TextMeshProUGUI progressText;
    public TextMeshProUGUI loadingText;

    public Chat chatController;

    [SerializeField]
    private MenuSoundManager menuSoundManagerClass;
    

    private MenuSoundManager menuSoundManager;
    #endregion

    void Awake()
    {
        // Only create one instance
        if (GameObject.Find(achievementCanvas.name+"(Clone)") == null)
            achievementCanvas = Instantiate(achievementCanvas);
        menuSoundManager = Instantiate(menuSoundManagerClass, transform.localPosition, transform.localRotation, this.transform);
        
    }

    private void Start()
    {
        // Check seen changelog
        if (!ClientConfigurationManager.Instance.clientConfiguration.SawChangelog)
            changelogPanel.SetActive(true);
    }



    #region Multiplayer

    public void UpdateRoomList()
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            //bool success = PhotonNetwork.GetCustomRoomList(TypedLobby.Default, "C0 = 0");

            //if (!success)
            //{
            //    print("Refreshing rooms through UpdateRoomlist failed. Ignore this for now.");
            //}
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

    #endregion



    #region Loading (singleplayer) scenes
    public void LoadLevel()
    {
        SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
        if (loadingScreen != null)
            StartCoroutine(LoadAsynchronously(ScenesInformation.sceneNames[SceneTitle.WASTELAND]));
        else
            SceneManager.LoadScene(ScenesInformation.sceneNames[SceneTitle.WASTELAND]);
    }

    public void LoadShipyard()
    {
        SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
        if (loadingScreen != null)
            StartCoroutine(LoadAsynchronously(ScenesInformation.sceneNames[SceneTitle.SHIPYARD]));
        else
            SceneManager.LoadScene(ScenesInformation.sceneNames[SceneTitle.SHIPYARD]);
    }

    public void LoadMainMenu()
    {
        SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
        SceneManager.LoadScene(ScenesInformation.sceneNames[SceneTitle.MAIN]);
    }

    public void LoadLobbyFromOtherScene()
    {
        // In case the game was paused
        Time.timeScale = 1f;
        PlayerNetwork.ReturnToMain();
        MainCanvasManager.instance.ShowPanel(PanelType.LOBBY);
    }

    #endregion

    #region Menu functions
    public void PlayButtonSound()
    {
        menuSoundManager.PlaySound(SoundType.CLICKBUTTON);
    }

    public void OpenFriendPanel(Animator anim)
    {
        bool open = anim.GetBool("open");
        anim.SetBool("open", !open);
    }

    public void OpenPopup(GameObject panel)
    {
        // Set active and put in front
        panel.SetActive(true);
        panel.transform.SetAsLastSibling();
    }

    public void ClosePanel(GameObject panel)
    {
        panel.SetActive(false);
    }

    public void SawChangelog()
    {
        ClientConfigurationManager.Instance.clientConfiguration.SawChangelog = true;
        ClientConfigurationManager.Instance.SavePlayerSettings();
    }

    public void ExitGame()
    {
        Application.Quit();
    }
    #endregion

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

    #region Unused
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
    #endregion
}
