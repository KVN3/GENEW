﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region Data Structs
[System.Serializable]
public struct PlayerRunData
{
    public int currentLap;
    public int maxLaps;
    public TimeSpan raceTime;
    public TimeSpan bestRaceTime;
    public TimeSpan totalTime;

    public List<TimeSpan> raceTimes;
    public List<GameObject> waypoints;

    public bool raceFinished;

    // Extra
    public float charges;
    public float score;
}
#endregion

[RequireComponent(typeof(Rigidbody))]
public class PlayerShip : Ship
{
    #region Initialize and Assign Variables
    public PlayerRunData runData;
  
    public override void Start()
    {
        base.Start();

        // Set currentlap, maxlaps, timer
        runData.currentLap = 0;
        runData.maxLaps = 3;
        runData.raceTime = TimeSpan.Parse("00:00:00.000");
        //runData.bestRaceTime = TimeSpan.Parse("00:01:47.222");
        runData.raceTimes = new List<TimeSpan>();
        runData.raceFinished = false;
        runData.waypoints = new List<GameObject>();
    }
    #endregion

    #region Collisions and Triggers
    //private void OnCollisionEnter(Collision other)
    //{
    //    if (!other.gameObject.CompareTag("OuterWall"))
    //    {
    //        audioSource.clip = alarmClip;
    //        audioSource.Play();
    //    }
    //}

    private void OnTriggerEnter(Collider other)
    {
        #region FinishLine
        if (other.gameObject.CompareTag("FinishLine"))
        {
            // Fixes an error when racetimes = null after restarting on Gianni's level
            if (runData.raceTimes == null)
                runData.raceTimes = new List<TimeSpan>();

            // Save lap if race has started (lap > 0)
            if (runData.currentLap > 0)
            {
                // Add laptime to racetimes if not finished
                if (!runData.raceFinished)
                    runData.raceTimes.Add(runData.raceTime);
                if (runData.bestRaceTime == TimeSpan.Parse("00:00:00"))
                    runData.bestRaceTime = runData.raceTime;
                else if (runData.raceTime < runData.bestRaceTime)
                    runData.bestRaceTime = runData.raceTime;
            }
            // If finished
            if (runData.currentLap == runData.maxLaps) // 3/3 laps + finish
            {
                Debug.Log("playerShip Finished");

                runData.raceFinished = true;
            }
            else // If not finished
            {
                Debug.Log($"playerShip Crossed Finish Line");

                // Reset Time. Only reset when lap > 0 
                if (runData.currentLap > 0 && runData.waypoints.Count > 0)
                {
                    runData.raceTime = TimeSpan.Parse("00:00:00.000");

                    runData.currentLap++;

                    // Reset waypoints
                    runData.waypoints.Clear();
                }

                // Add a lap
                if (runData.currentLap == 0)
                    runData.currentLap++;
            }
        }
        #endregion

        #region Waypoints

        if (other.gameObject.CompareTag("Waypoint"))
        {
            Debug.Log("Waypoint passed!");
            //other.GetComponent<Waypoint>().activated = true;
            runData.waypoints.Add(other.gameObject);
        }

        #endregion
    }
    #endregion
}

// Apply boundaries
//if (transform.position.x > ShipComponents.rightBound && force.x > 0)
//{
//    rb.velocity = new Vector3(0f, force.y, force.z);
//}

//else if (transform.position.x < ShipComponents.leftBound && force.x < 0)
//{
//    rb.velocity = new Vector3(0f, force.y, force.z);
//}
//else
//    rb.AddForce(force);

//float verticalInput = Input.GetAxis("Vertical");

//if (verticalInput < 0)
//{
//    //rb.drag = verticalInput * -10;
//    //rb.angularDrag = verticalInput * 10;
//}
//else
//{
//    rb.drag = 0;
//    rb.angularDrag = 0;
//    float correctedInput = verticalInput;
//    float currentAngleY = transform.rotation.eulerAngles.y;

//    if (currentAngleY >= 0 && currentAngleY < 90 || currentAngleY >= 270 && currentAngleY <= 360)
//    {
//        correctedInput = verticalInput * -1;
//    }

//    Vector3 forward = correctedInput * transform.forward * Time.deltaTime * ShipComponents.movementSpeedFactor;


//    rb.AddRelativeForce(forward);
//}

//public void Shoot()
//{
//    //Select random bullet type
//    //int bulletIndex = Random.Range(0, bulletData.bullets.Length);
//    //Bullet bullet = bulletData.bullets[bulletIndex];
//    //bullet.isEnemyBullet = false;

//    //Spawn the bullet
//    //Instantiate(bullet, this.transform.position, this.transform.rotation);
//}

// Force andere kant drag implementeren 
// Voor de rotatie toepassing velocity oohale n als vfactor inverse transform je hem door de roatie van de tras=ns
// Krijg local snelheid, recht naar voren beweegt z (LOOKAAL) heel groot z y bijna 0
// Pas rotatie toe en vervolgens trans je velocity door nieuwe rotatie heen
// Krijg nieuwe vector3 worldspace, apply rb
// Lokaal altijd z