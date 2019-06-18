using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;
using UnityEngine.UI;

public enum FlashColor
{
    RED, BLUE
}

public class HUD : MyMonoBehaviour
{
    public static HUD Instance;

    public PlayerShip PlayerShip { get; set; }

    public GameObject InGamePanel;
    public GameObject RaceStartPanel;
    public GameObject RaceEndPanel;
    public GameObject PauseScreen;

    [SerializeField]
    private GameObject blackBackground;

    [SerializeField]
    private GameObject redBackground;

    [SerializeField]
    private GameObject blueBackground;

    [SerializeField]
    private GameObject spectatingText;

    Animator anim;
    CanvasGroup canvasGroup;

    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;
        Assert.IsNotNull(InGamePanel);
        Assert.IsNotNull(RaceEndPanel);

        anim = GetComponent<Animator>();

        RaceEndPanel.SetActive(false);
        RaceEndPanel.GetComponent<RaceEndScreenManager>().PlayerShip = PlayerShip;
    }

    void Start()
    {
        //PlayerShip.OnPlayerFinishedRaceNotifyUIDelegate = (bool spectating) =>
        //{
        //    PlayerFinishedRaceEvent(spectating);
        //};

        PlayerShip.components.system.OnPlayerShipHitDelegate = (float durationInSeconds, FlashColor flashColor) =>
        {
            StartCoroutine(C_ScreenFlash(durationInSeconds, flashColor));
        };
    }

    private void Update()
    {
        // Pause
        if (Input.GetKeyDown(KeyCode.Escape))
            TogglePause();

        // Pause screen (should be a unityEvent/delegate)
        if (GameConfiguration.isPaused && !PauseScreen.activeSelf)
        {
            if (PhotonNetwork.PlayerList.Length == 1)
                Time.timeScale = 0f;
            PauseScreen.SetActive(true);
        }
        if (!GameConfiguration.isPaused)
        {
            if (PhotonNetwork.PlayerList.Length == 1)
                Time.timeScale = 1f;
            PauseScreen.SetActive(false);
        }

        // Leaving match
        if (Input.GetKeyDown(KeyCode.P))
        {
            PlayerNetwork.ReturnToLobby();
        }
    }

    #region RACE FINISHED
    public void PlayerFinishedRaceEvent(bool spectating)
    {
        if (spectating)
        {
            print("UI PLAYER FINISHED EVENT: SPECTATING");
        }
        else
        {
            print("UI PLAYER FINISHED EVENT: SETBG");
            blackBackground.SetActive(true);
            spectatingText.SetActive(false);
        }

        InGamePanel.GetComponent<CanvasGroup>().alpha = 0f;
        RaceEndPanel.SetActive(true);
        anim.SetTrigger("RaceFinished");
    }
    #endregion

    #region DAMAGE FLASH
    // Flash screen in given color for given amount of time (must link)
    private IEnumerator C_ScreenFlash(float durationInSeconds, FlashColor flashColor)
    {
        GameObject screen = GetScreen(flashColor);

        screen.SetActive(true);
        yield return new WaitForSeconds(durationInSeconds);
        screen.SetActive(false);
    }

    private GameObject GetScreen(FlashColor color)
    {
        GameObject screen = null;

        switch (color)
        {
            case FlashColor.RED:
                screen = redBackground;
                break;
            case FlashColor.BLUE:
                screen = blueBackground;
                break;
        }

        return screen;
    }
    #endregion

    public void TogglePause()
    {
        GameConfiguration.isPaused = !GameConfiguration.isPaused;
    }
}
