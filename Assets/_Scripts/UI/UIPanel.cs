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
    public TextMeshProUGUI raceTimeText;
    public TextMeshProUGUI bestRaceTimeText;

    public TextMeshProUGUI raceLapText;
    public TextMeshProUGUI speedText;

    public Slider speedMeter;

    public Slider chargeBar;

    public TextMeshProUGUI raceTimesText;

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

    protected override void Start()
    {
        base.Start();

        // Default text
        raceLapText.text = $"Lap: 0/3";
    }

    void Update()
    {
        #region In-GameUI

        // Laps
        raceLapText.text = $"{LocalizationService.Instance.GetTextByKey("LAP")}: {playerShip.runData.currentLap}/{playerShip.runData.maxLaps}";

        // Race Time
        raceTimeText.text = LocalizationService.Instance.GetTextByKey("CURRENT_TIME") + " - " + playerShip.runData.raceTime.ToString(@"mm\:ss\.ff");

        // Best race time
        if (playerShip.runData.bestRaceTime == TimeSpan.Parse("00:00:00.000"))
            bestRaceTimeText.text = "--.---";
        else
            bestRaceTimeText.text = playerShip.runData.bestRaceTime.ToString(@"mm\:ss\.ff");


        // Speed (Get and convert speed from rb)

        //Rigidbody rb = target.GetComponent<Rigidbody>();
        //var localVelocity = transform.InverseTransformVector(rb.velocity);
        //var forwardSpeed = Mathf.Abs(localVelocity.z);
        //playerSpeedText.text = forwardSpeed.ToString("0") + " KM/H";
        float currSpeed = playerShip.components.movement.GetCurrentSpeed();
        //playerSpeedText.text = currSpeed.ToString("0") + " KM/H";

        // Speed
        speedText.text = currSpeed.ToString("0");
        speedMeter.value = (currSpeed / playerShip.components.movement.GetCurrentMaxSpeed()) * 1;

        // Charges
        chargeBar.value =
            (playerShip.components.forcefield.GetCharges() / playerShip.components.forcefield.maxCharges) * 1;

        #endregion

        #region Race Finished Screen
        // Only show racetimes when finished and display them using a stringbuilder (for lines)
        if (playerShip.runData.raceFinished)
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < playerShip.runData.raceTimes.Count; i++)
            {
                string lapTime = playerShip.runData.raceTimes[i].ToString(@"mm\:ss\.ff");
                string lapCount = (i + 1).ToString(); // Arrays start at 0 but laps start at 1

                builder.Append("Lap ").Append(lapCount).Append(": ").Append(lapTime).AppendLine();
            }
            builder.Append("Best Lap: ").Append(playerShip.runData.bestRaceTime.ToString(@"mm\:ss\.ff"));

            raceTimesText.text = builder.ToString();
        }
        #endregion
    }
}