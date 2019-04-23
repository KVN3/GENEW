using Photon.Pun;
using Photon.Realtime;
using System;
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
    public List<TimeSpan> leaderboardTimes;

    public int currentPos;

    public List<TimeSpan> raceTimes;
    public int currentWaypoint;
    public bool isOverHalfway;
    public bool isWrongWay;
    public bool raceStarted;
    public bool isLastLap;

    public bool raceFinished;

    // Extra
    public float charges;
    //public float score;
}
#endregion

[RequireComponent(typeof(Rigidbody))]
public class PlayerShip : Ship
{
    #region Initialize and Assign Variables
    public PlayerRunData runData;

    public Player remoteData;
    //public PlayerController playerControllerClass;

    //private PlayerController pc;

    public override void Awake()
    {
        base.Awake();

        //pc = Instantiate(playerControllerClass, transform.position, Quaternion.identity, transform);
        //pc.playerShip = this;
    }

    public override void Start()
    {
        base.Start();

        InitRaceData();
        StartCoroutine(SubstractScore());
    }
    #endregion

    private void Update()
    {
        if (GetComponent<PhotonView>().IsMine)
        {
            if (!runData.raceFinished && RaceManager.raceStarted)
                runData.raceTime = runData.raceTime.Add(TimeSpan.FromSeconds(1 * Time.deltaTime));

            //if (Input.GetKeyDown(KeyCode.Space))
            //{
            //    Ammo--;

            //    OnAmmoChanged(Ammo);
            //}

            //if (Input.GetButtonDown("Fire1"))
            //{
            //    Debug.Log("Button");
            //}
        }
    }

    IEnumerator SubstractScore()
    {
        while (true && !runData.raceFinished)
        {
            yield return new WaitForSeconds(1);
        }
    }

    private void InitRaceData()
    {
        // Set currentlap, maxlaps, timer, pos
        runData.currentLap = 0;
        runData.maxLaps = 1; // Should be configurable by variable
        runData.raceTime = TimeSpan.Parse("00:00:00.000");
        //runData.bestRaceTime = TimeSpan.Parse("00:01:47.222");
        runData.raceTimes = new List<TimeSpan>();
        // if PlayerPrefs.HasKey(leaderboard) { }
        runData.leaderboardTimes = new List<TimeSpan>();
        runData.currentPos = 1;

        // Prevents cheating times
        runData.raceFinished = false;
        runData.isOverHalfway = false;
        // Guidance
        runData.isWrongWay = false;
        runData.isLastLap = false;
    }

    #region Collisions and Triggers

    new protected void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("DangerTrigger"))
        {
            aiSoundManager.PlayDangerSound(SoundType.AIDANGER);
        }

        #region FinishLine
        if (other.gameObject.CompareTag("FinishLine"))
        {
            // Reset waypoint count 
            runData.currentWaypoint = -1;

            // Fixes an error when racetimes = null after restarting on Gianni's level
            if (runData.raceTimes == null)
                runData.raceTimes = new List<TimeSpan>();

            // Save lap if race has started (lap > 0) and qualified (isOverHalfWay) lap 
            if (runData.currentLap > 0 && runData.isOverHalfway)
            {
                // Add laptime to racetimes if not finished
                if (!runData.raceFinished)
                    runData.raceTimes.Add(runData.raceTime);

                // Update best time if not set (00:00:00)
                if (runData.bestRaceTime == TimeSpan.Parse("00:00:00"))
                    runData.bestRaceTime = runData.raceTime;

                // Else update best time if current time is faster
                else if (runData.raceTime < runData.bestRaceTime)
                    runData.bestRaceTime = runData.raceTime;
            }
            // If finished
            if (runData.currentLap == runData.maxLaps && runData.isOverHalfway) // 3/3 laps + finish
            {
                Debug.Log("playerShip Finished");
                levelSoundManager.PlaySound(SoundType.VICTORY);

                foreach (TimeSpan time in runData.raceTimes)
                    runData.totalTime += time;

                runData.leaderboardTimes.Add(runData.bestRaceTime);

                runData.raceFinished = true;
            }
            else // If not finished
            {
                // Reset Time. Only reset when lap > 0 && over half way
                if (runData.currentLap > 0 && runData.isOverHalfway)
                {
                    Debug.Log($"PlayerShip Crossed Finish Line");
                    levelSoundManager.PlaySound(SoundType.LAPPASSED);

                    runData.raceTime = TimeSpan.Parse("00:00:00.000");

                    runData.currentLap++;

                    // Reset halfwaypoint
                    runData.isOverHalfway = false;
                }

                // Add a lap at start
                if (runData.currentLap == 0)
                    runData.currentLap++;
            }
        }
        #endregion

        #region Waypoints

        if (other.gameObject.CompareTag("Waypoint"))
        {
            // If passed previous waypoint
            if (other.GetComponent<Waypoint>().number < runData.currentWaypoint)
            {
                Debug.Log("Wrong Way!");
                runData.isWrongWay = true;
            }
            else // update with new waypoint
            {
                Debug.Log($"Waypoint updated! ({other.GetComponent<Waypoint>().number})");
                runData.currentWaypoint = other.GetComponent<Waypoint>().number;
                runData.isWrongWay = false;
                if (other.GetComponent<Waypoint>().isHalfwayPoint)
                    runData.isOverHalfway = true;
            }

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