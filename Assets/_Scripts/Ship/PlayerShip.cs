using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

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
    public bool hasNewBestTime;
    public bool isLastLap;

    public bool raceFinished;

    // Replay data
    public List<float> positionsX;
    public List<float> positionsY;
    public List<float> positionsZ;
    public List<Quaternion> replayRotations;
    public List<Vector3> replayPositions;

    // Extra
    public float charges;
}
#endregion

[RequireComponent(typeof(Rigidbody))]
public class PlayerShip : Ship
{
    #region Initialize and Assign Variables
    public PlayerRunData runData;

    protected PlayerCamera playerCamera;
    public UnityAction<int, TimeSpan, double, string> OnPlayerFinishedRaceDelegate;

    Dictionary<string, Dictionary<string, TimeSpan>> playerTimes;

    public PhotonView GetPhotonView()
    {
        return photonView;
    }

    public override void Awake()
    {
        base.Awake();

        photonView = GetComponent<PhotonView>();

        // Destroy/disable stuff only needed for main player
        if (!photonView.IsMine)
        {
            Destroy(GetComponent<AudioListener>());
        }

    }

    public void Start()
    {
        InitRaceData();

        if (!photonView.IsMine)
        {
            aiSoundManager.enabled = false;
            levelSoundManager.enabled = false;
        }
    }
    #endregion

    private void Update()
    {
        if (!runData.raceFinished && RaceManager.raceStarted)
            runData.raceTime = runData.raceTime.Add(TimeSpan.FromSeconds(1 * Time.deltaTime));

        runData.positionsX.Add(transform.position.x);
        runData.positionsY.Add(transform.position.y);
        runData.positionsZ.Add(transform.position.z);
        runData.replayRotations.Add(transform.rotation);
    }

    private void InitRaceData()
    {
        // Set currentlap, maxlaps, timer, pos
        runData.currentLap = 0;
        runData.maxLaps = 1; // Should be configurable by variable
        runData.raceTime = TimeSpan.Parse("00:00:00.000");
        //runData.bestRaceTime = TimeSpan.Parse("00:01:47.222");
        runData.raceTimes = new List<TimeSpan>();
        runData.leaderboardTimes = new List<TimeSpan>();
        runData.currentPos = 1;

        // Prevents cheating times
        runData.raceFinished = false;
        runData.isOverHalfway = false;
        // Guidance
        runData.isWrongWay = false;
        runData.isLastLap = false;
        runData.hasNewBestTime = false;

        // Replay data
        runData.positionsX = new List<float>();
        runData.positionsY = new List<float>();
        runData.positionsZ = new List<float>();
        runData.replayRotations = new List<Quaternion>();
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

            // Check for valid lap
            if (!runData.isWrongWay && runData.isOverHalfway) 
            {
                // Save lap if race has started (lap > 0) // THIS ALWAYS HAPPENS REGARDLESS OF FINISHED OR NOT
                if (runData.currentLap > 0)
                {
                    // Add laptime to racetimes if not finished
                    if (!runData.raceFinished)
                        runData.raceTimes.Add(runData.raceTime);

                    // Update best time if not set (00:00:00)
                    if (runData.bestRaceTime == TimeSpan.Parse("00:00:00"))
                    {
                        runData.bestRaceTime = runData.raceTime;
                        runData.hasNewBestTime = true;
                    }

                    // Else update best time if current time is faster
                    else if (runData.raceTime < runData.bestRaceTime)
                    {
                        runData.bestRaceTime = runData.raceTime;
                        runData.hasNewBestTime = true;
                    }

                    // Achievements
                    if (SceneManager.GetActiveScene().name == "Large Wasteland")
                    {
                        if (runData.raceTime < TimeSpan.Parse("00:00:50.000"))
                            AchievementManager.UpdateAchievement(0, 1f);
                        if (runData.raceTime < TimeSpan.Parse("00:00:45.000"))
                            AchievementManager.UpdateAchievement(1, 1f);
                        if (runData.raceTime < TimeSpan.Parse("00:00:40.000"))
                            AchievementManager.UpdateAchievement(2, 1f);
                        if (runData.raceTime < TimeSpan.Parse("00:00:35.000"))
                            AchievementManager.UpdateAchievement(3, 1f);
                    }
                }

                // If finished
                if (runData.currentLap == runData.maxLaps) // 3/3 laps + finish
                {
                    Debug.Log("playerShip Finished");
                    levelSoundManager.PlaySound(SoundType.VICTORY);

                foreach (TimeSpan time in runData.raceTimes)
                {
                    runData.totalTime += time;
                }
                    
                    foreach (TimeSpan time in runData.raceTimes)
                        runData.totalTime += time;

                    // Leaderboard
                    HighscoreManager highscoreManager = new HighscoreManager();
                    Account account = Registration.GetCurrentAccount();
                    highscoreManager.AddHighscoreEntry(account.username, runData.bestRaceTime.ToString(), SceneManager.GetActiveScene().name);

                    // Update bestTime for UI
                    if (!PlayerPrefs.HasKey("Highscore"))
                        PlayerPrefs.SetString("Highscore", runData.bestRaceTime.ToString());
                    else
                    {
                        if (TimeSpan.Parse(PlayerPrefs.GetString("Highscore")) > runData.bestRaceTime)
                            PlayerPrefs.SetString("Highscore", runData.bestRaceTime.ToString());
                    }

                    // Save replay
                    SaveReplay();

                runData.raceFinished = true;

                // Analytics
                double averageLapTime = runData.totalTime.TotalSeconds / runData.maxLaps;
                string playerName = "Anon";

                if (PhotonNetwork.IsConnected)
                    playerName = PhotonNetwork.LocalPlayer.NickName;

                OnPlayerFinishedRaceDelegate(runData.maxLaps, runData.totalTime, averageLapTime, playerName);
            }
            else // If not finished
            {
                // Reset Time. Only reset when lap > 0 && over half way
                if (runData.currentLap > 0 && runData.isOverHalfway)
                {
                    // Reset Time. Only reset when lap > 0
                    if (runData.currentLap > 0)
                    {
                        Debug.Log($"PlayerShip Crossed Finish Line");
                        levelSoundManager.PlaySound(SoundType.LAPPASSED);

                        // Save replay
                        SaveReplay();

                        // Reset stuff
                        runData.raceTime = TimeSpan.Parse("00:00:00.000");
                        runData.isOverHalfway = false;

                        runData.currentLap++;
                    }
                }
            }

            if (runData.currentLap == 0)
                runData.currentLap++;
        }
            #endregion

            #region Waypoints

            if (other.gameObject.CompareTag("Waypoint"))
        {
            // If passed previous waypoint
            if (other.GetComponent<Waypoint>().number < runData.currentWaypoint)
            {
                // Debug.Log("Wrong Way!");
                runData.isWrongWay = true;
            }
            else // update with new waypoint
            {
                // Debug.Log($"Waypoint updated! ({other.GetComponent<Waypoint>().number})");
                runData.currentWaypoint = other.GetComponent<Waypoint>().number;
                runData.isWrongWay = false;
                if (other.GetComponent<Waypoint>().isHalfwayPoint)
                    runData.isOverHalfway = true;
            }

        }
        #endregion
    }
    #endregion

    #region GetSet
    public void SetPlayerCamera(PlayerCamera playerCamera)
    {
        this.playerCamera = playerCamera;
    }

    public PlayerCamera GetPlayerCamera()
    {
        return playerCamera;
    }
    #endregion

    public void SaveReplay()
    {
        float[] arrayX = runData.positionsX.ToArray();
        float[] arrayY = runData.positionsY.ToArray();
        float[] arrayZ = runData.positionsZ.ToArray();
        Quaternion[] arrayQ = runData.replayRotations.ToArray();

        ReplayManager replayManager = new ReplayManager();
        replayManager.SaveReplay(Environment.UserName, SceneManager.GetActiveScene().name, runData.positionsX, runData.positionsY, runData.positionsZ, runData.replayRotations, runData.raceTime.ToString());

        //Destroy(replayManager);
        //PlayerPrefsX.SetFloatArray("ReplayX", arrayX);
        //PlayerPrefsX.SetFloatArray("ReplayY", arrayY);
        //PlayerPrefsX.SetFloatArray("ReplayZ", arrayZ);
        //PlayerPrefsX.SetQuaternionArray("ReplayQ", arrayQ);
    }
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