using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Localization;

public class UIPanel : UIBehaviour
{
    #region Fields
    [Header("Lap text")]
    public TextMeshProUGUI lapText;
    public TextMeshProUGUI currentLapText;
    public TextMeshProUGUI lapSeperatorText;
    public TextMeshProUGUI lastLapText;

    [Header("Time text")]
    public TextMeshProUGUI raceTimeText;
    public TextMeshProUGUI bestRaceTimeText;

    [Header("Pos text")]
    public Image posBg;
    public TextMeshProUGUI posText;
    public TextMeshProUGUI currentPosText;
    public TextMeshProUGUI posSeperatorText;
    public TextMeshProUGUI lastPosText;

    [Header("Speed")]
    public TextMeshProUGUI speedText;
    public Image speedBg;
    public Sprite speedBgSprite;
    public Sprite speedBgSpriteActivated;
    public Slider speedMeter;
    public Image speedMeterFilling;
    public Color speedMeterBoostedColor;
    public Color speedMeterColor;

    public Slider boostMeter;

    [Header("Extra")]
    public Image itemBox;

    [Header("ChargeBar")]
    public Slider chargeBar;
    public Image chargeBarFilling;

    [Header("RaceEndScreen")]
    public TextMeshProUGUI raceTimesText;

    #region properties
    // Non-UI
    public int playerCount { get; set; }

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
    #endregion

    protected override void Start()
    {
        base.Start();

        // Turn on/off position UI
        if (playerCount > 1)
            posBg.GetComponent<CanvasGroup>().alpha = 1f;
        else
            posBg.GetComponent<CanvasGroup>().alpha = 0f;

        boostMeter.value = 0;
    }

    void Update()
    {
        LocalizationService ls = LocalizationService.Instance;
        PlayerRunData pd = playerShip.runData;

        #region In-GameUI

        // Laps
        lapText.text = ls.GetTextByKey("LAP");
        currentLapText.text = pd.currentLap.ToString();
        lapSeperatorText.text = "/";
        lastLapText.text = pd.maxLaps.ToString();

        // Position(rank)
        posText.text = ls.GetTextByKey("POS");
        currentPosText.text = pd.currentPos.ToString();
        posSeperatorText.text = "/";
        lastPosText.text = playerCount.ToString();

        // Race Time
        raceTimeText.text = ls.GetTextByKey("CURRENT_TIME") + " - " + pd.raceTime.ToString(@"mm\:ss\.ff");

        // Best race time
        if (pd.bestRaceTime == TimeSpan.Parse("00:00:00.000"))
            bestRaceTimeText.text = "--.---";
        else
            bestRaceTimeText.text = pd.bestRaceTime.ToString(@"mm\:ss\.ff");

        // Speed
        if (playerShip.components.movement.GetCurrentSpeed() > 1f)
            speedBg.sprite = speedBgSpriteActivated;
        else
            speedBg.sprite = speedBgSprite;

        float currSpeed = playerShip.components.movement.GetCurrentSpeed() * 2;
        speedText.text = currSpeed.ToString("0");
        speedMeter.value = (currSpeed / playerShip.components.movement.GetCurrentMaxSpeed()) * 0.6f; // Compensate with * 2 and make slightly higher so it sticks to 100% when a bit less than maxspeed is reached
        if (playerShip.components.movement.IsBoosted())
        {
            
            if (boostMeter.value <= 1)
                boostMeter.value += Time.deltaTime;
            //speedMeterFilling.color = speedMeterBoostedColor;
            speedBg.color = speedMeterBoostedColor;
        }
        else
        {
            if (boostMeter.value >= 0)
                boostMeter.value -= Time.deltaTime;
            //speedMeterFilling.color = speedMeterColor;
            speedBg.color = speedMeterColor;
        }

        // Charges
        chargeBar.value = (playerShip.components.forcefield.GetCharges() / playerShip.components.forcefield.maxCharges) * 1;

        // Item
        if (playerShip.GetItemAmount() > 0)
        {
            // Change Alpha
            Color tempColor = itemBox.color;
            tempColor.a = 1f;
            itemBox.color = tempColor;
        }
        else
        {
            Color tempColor = itemBox.color;
            tempColor.a = 0f;
            itemBox.color = tempColor;
        }

        #endregion

        #region Race Finished Screen
        // Only show racetimes when finished and display them using a stringbuilder (for lines)
        if (pd.raceFinished)
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < pd.raceTimes.Count; i++)
            {
                string lapTime = pd.raceTimes[i].ToString(@"mm\:ss\.ff");
                string lapCount = (i + 1).ToString(); // Arrays start at 0 but laps start at 1

                builder.Append(ls.GetTextByKey("LAP")).Append(" ").Append(lapCount).Append(": ").Append(lapTime).AppendLine();
            }
            builder.AppendLine().Append(ls.GetTextByKey("BEST_LAPTIME")).Append(": ").Append(pd.bestRaceTime.ToString(@"mm\:ss\.ff"));

            raceTimesText.text = builder.ToString();
        }
        #endregion
    }
}