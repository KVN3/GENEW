using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;
using UnityEngine.UI;

public class HUD : MyMonoBehaviour, IObserver
{
    public PlayerShip PlayerShip { get; set; }

    public GameObject InGamePanel;
    public GameObject RaceStartPanel;
    public GameObject RaceEndPanel;

    [SerializeField]
    private GameObject blackBackground;

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
    }

    private void Update()
    {
        // Leaving match
        if (Input.GetKeyDown(KeyCode.P))
        {
            PlayerNetwork.ReturnToMain();
            MainCanvasManager.instance.ShowPanel(PanelType.LOBBY);
        }
    }

    private void PlayerFinishedRaceEvent(bool spectating)
    {
        if (spectating)
        {

        }
        else
        {
            blackBackground.SetActive(true);
        }

        InGamePanel.SetActive(false);
        RaceEndPanel.SetActive(true);
        anim.SetTrigger("RaceFinished");
    }

    public void OnNotify(float score, float charges)
    {
        // Do Something

    }
}
