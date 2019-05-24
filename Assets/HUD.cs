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

public class HUD : MyMonoBehaviour, IObserver
{
    public PlayerShip PlayerShip { get; set; }

    public GameObject InGamePanel;
    public GameObject RaceStartPanel;
    public GameObject RaceEndPanel;

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
        Assert.IsNotNull(PlayerShip);
        Assert.IsNotNull(InGamePanel);
        Assert.IsNotNull(RaceEndPanel);

        anim = GetComponent<Animator>();

        RaceEndPanel.SetActive(false);
    }

    void Start()
    {
        PlayerShip.OnPlayerFinishedRaceNotifyUIDelegate = (bool spectating) =>
        {
            PlayerFinishedRaceEvent(spectating);
        };

        PlayerShip.OnPlayerShipHitDelegate = (float durationInSeconds, FlashColor flashColor) =>
        {
            StartCoroutine(C_ScreenFlash(durationInSeconds, flashColor));
        };
    }

    private void Update()
    {
        // Leaving match
        if (Input.GetKeyDown(KeyCode.P))
        {
            PlayerNetwork.ReturnToLobby();

        }
    }

    #region RACE FINISHED
    private void PlayerFinishedRaceEvent(bool spectating)
    {
        if (spectating)
        {

        }
        else
        {
            blackBackground.SetActive(true);
            spectatingText.SetActive(false);
        }

        InGamePanel.SetActive(false);
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




    public void OnNotify(float score, float charges)
    {
        // Do Something
    }
}
