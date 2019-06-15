using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIPanel : UIBehaviour
{
    #region Fields
    public GameObject nonTutorialPanels;
    public GameObject tutorialPanels;

    [Header("Lap text")]
    public TextMeshProUGUI lapText;
    public TextMeshProUGUI currentLapText;
    public TextMeshProUGUI lapSeperatorText;
    public TextMeshProUGUI lastLapText;

    [Header("Time text")]
    public TextMeshProUGUI currentText;
    public TextMeshProUGUI raceTimeText;
    public TextMeshProUGUI bestTimeText;
    public TextMeshProUGUI bestRaceTimeText;

    [Header("Pos text")]
    public GameObject posBg;
    public TextMeshProUGUI posText;
    public TextMeshProUGUI currentPosText;
    public TextMeshProUGUI posSeperatorText;
    public TextMeshProUGUI lastPosText;

    [Header("Speed")]
    public TextMeshProUGUI speedText;
    public TextMeshProUGUI speedUnitText;
    public Image speedBg;
    public Sprite speedBgSprite;
    public Sprite speedBgSpriteActivated;
    public Slider speedMeter;
    public Image speedMeterFilling;
    public Color speedMeterBoostedColor;
    public Color speedMeterColor;

    public Slider boostMeter;

    [Header("Item")]
    public GameObject itemBox;
    public Image item;
    public TextMeshProUGUI itemAmount;
    public TextMeshProUGUI itemText;

    [Header("ChargeBar")]
    public Slider chargeBar;
    public Image chargeBarFilling;
    public GameObject wrongWayPanel;
    public TextMeshProUGUI wrongWayText;

    [Header("RaceEndScreen")]
    public TextMeshProUGUI raceEndScreenText;
    public TextMeshProUGUI endTimeText;
    public TextMeshProUGUI leaderboardText;
    public GameObject entryTemplate;
    public GameObject leaderboardContent;
    public TextMeshProUGUI spectatingText;
    public TextMeshProUGUI rankText;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI returnToLobbyText;
    private bool initialisedScoreBoard;


    [Header("RaceStartScreen")]
    public TextMeshProUGUI countDownText;

    [Header("CountDown")]
    public CountDownController countDownController;

    [Header("Map")]
    private Map map;
    #endregion

    #region Properties
    // Properties
    public int PlayerCount { get; set; }

    public PlayerShip playerShip
    {
        get
        {
            HUD Hud = GetComponentInParent<HUD>();

            Assert.IsNotNull(Hud, "Bla bla je hebt het verkeerd ingesteld");
            Assert.IsNotNull(Hud.PlayerShip, "Je hebt geen playerShip in je hud");

            return Hud.PlayerShip;
        }
    }
    #endregion

    protected override void Start()
    {
        base.Start();

        // Turn on/off position UI
        if (PhotonNetwork.PlayerList.Length > 1)
            posBg.SetActive(true); 
        else
            posBg.SetActive(false);

        initialisedScoreBoard = false;
        boostMeter.value = 0;
        countDownController = FindObjectOfType<CountDownController>();

        RectTransform transform = nonTutorialPanels.GetComponent<RectTransform>();
        RectTransform tutorialPanelsTransform = tutorialPanels.GetComponent<RectTransform>();
        if (SceneManager.GetActiveScene().name.Equals("Tutorial"))
        {
            
            //transform.localPosition = new Vector2(9999f, 0); // werkt niet
            // tutorialPanelsTransform.localPosition = new Vector2(0,0); 
            nonTutorialPanels.SetActive(false);
        }
        else
        {
            //transform.localPosition = new Vector2(0,0); 
            //tutorialPanelsTransform.localPosition = new Vector2(9999f, 0);
            tutorialPanels.SetActive(false);
        }
    }

    void Update()
    {
        PlayerRunData pd = playerShip.runData;

        #region In-GameUI

        // Laps
        lapText.text = LocalizationManager.GetTextByKey("LAP")+": ";
        currentLapText.text = pd.currentLap.ToString();
        lapSeperatorText.text = "/";
        lastLapText.text = pd.maxLaps.ToString();

        // Position(rank)
        posText.text = LocalizationManager.GetTextByKey("POS");
        currentPosText.text = pd.currentPos.ToString();
        posSeperatorText.text = "/";
        lastPosText.text = PlayerCount.ToString();

        // Race Time
        currentText.text = LocalizationManager.GetTextByKey("CURRENT_TIME") + ": ";
        raceTimeText.text = pd.raceTime.ToString(@"mm\:ss\.ff");

        // Best race time
        bestTimeText.text = LocalizationManager.GetTextByKey("BEST") + ": ";
        if (PlayerPrefs.HasKey("Highscore"))
            bestRaceTimeText.text = TimeSpan.Parse(PlayerPrefs.GetString("Highscore")).ToString(@"mm\:ss\.ff");
        else
            bestRaceTimeText.text = "--.---";

        // Speed
        if (playerShip.components.movement.GetCurrentSpeed() > 1f)
            speedBg.sprite = speedBgSpriteActivated;
        else
            speedBg.sprite = speedBgSprite;

        float currSpeed = playerShip.components.movement.GetCurrentSpeed() * 2;
        speedText.text = currSpeed.ToString("0");
        speedMeter.value = (currSpeed / playerShip.components.movement.GetCurrentMaxSpeed()) * 0.6f; // Compensate with * 2 and make slightly higher so it sticks to 100% when a bit less than maxspeed is reached
        speedUnitText.text = LocalizationManager.GetTextByKey("SPEEDUNIT");
        // Boosted
        if (playerShip.components.movement.IsBoosted())
        {
            if (boostMeter.value <= 1)
                boostMeter.value += Time.deltaTime;
            speedBg.color = speedMeterBoostedColor;
        }
        else
        {
            if (boostMeter.value >= 0)
                boostMeter.value -= Time.deltaTime;
            speedBg.color = speedMeterColor;
        }

        // Achievement
        if (currSpeed > 600f)
            AchievementManager.UpdateAchievementByName("Speed demon", 1f);

        // Charges
        chargeBar.value = (playerShip.components.forcefield.GetCharges() / playerShip.components.forcefield.maxCharges) * 1;

        // Item
        if (playerShip.GetItemAmount() > 0)
        {
            itemBox.SetActive(true);
            item.sprite = playerShip.GetItem().sprite;
            itemAmount.text = playerShip.GetItemAmount().ToString();

            // Determine item for explanation
            if (playerShip.GetItem().GetType() == typeof(JammerMine))
                itemText.text = LocalizationManager.GetTextByKey("USE_ITEM_MINE");
            else if (playerShip.GetItem().GetType() == typeof(JammerProjectile))
                itemText.text = LocalizationManager.GetTextByKey("USE_ITEM_MISSILE");
            else if (playerShip.GetItem().GetType() == typeof(SmokeScreenItem))
                itemText.text = LocalizationManager.GetTextByKey("USE_ITEM_SMOKE");
            else if (playerShip.GetItem().GetType() == typeof(ForcefieldItem))
                itemText.text = LocalizationManager.GetTextByKey("USE_ITEM_BARRIER");
            else if (playerShip.GetItem().GetType() == typeof(SpeedBurst))
                itemText.text = LocalizationManager.GetTextByKey("USE_ITEM_BOOST");
        }
        else
        {
            itemBox.SetActive(false);
        }

        // Pop ups
        // Wrong Way
        if (pd.isWrongWay)
            wrongWayPanel.SetActive(true);
        else
            wrongWayPanel.SetActive(false);

        #endregion

        #region Race End Screen
        // Only show racetimes when finished
        if (pd.raceFinished)
        {
            // Title
            raceEndScreenText.text = LocalizationManager.GetTextByKey("RACE_RESULTS");

            endTimeText.text = LocalizationManager.GetTextByKey("TOTAL_TIME") + ": " + pd.raceTime.ToString(@"mm\:ss\.ff");

            // Leaderboard
            leaderboardText.text = LocalizationManager.GetTextByKey("LEADERBOARD");

            if (!initialisedScoreBoard)
            {
                HighscoreManager.Instance.ShowHighscores(SceneManager.GetActiveScene().name, entryTemplate, leaderboardContent);
                initialisedScoreBoard = true;
            }

            spectatingText.text = LocalizationManager.GetTextByKey("SPECTATING_NEXT");
            rankText.text = LocalizationManager.GetTextByKey("RANK");
            nameText.text = LocalizationManager.GetTextByKey("NAME");
            timeText.text = LocalizationManager.GetTextByKey("TIME");
            returnToLobbyText.text = LocalizationManager.GetTextByKey("RETURN_TO_LOBBY");
        }
        #endregion

        #region Race Start Screen

        if (countDownController != null && SceneManager.GetActiveScene().name != ScenesInformation.sceneNames[SceneTitle.TUTORIAL]) // and if !tutorial level
            countDownText.text = countDownController.CountDownText;
        else
            countDownText.text = "";

        #endregion

    }
}

// TODO: FLEXIBLE PANEL CREATING WITH Coroutine/AUTODESTROY