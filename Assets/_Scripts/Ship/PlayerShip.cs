using Photon.Pun;
using Photon.Realtime;
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
    public System.TimeSpan raceTime;
    public System.TimeSpan bestRaceTime;
    public List<System.TimeSpan> leaderboardTimes;

    public int currentPos;

    public int currentWaypoint;
    public Transform lastWaypoint;
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
    public UnityAction<int, System.TimeSpan, double, string> OnPlayerFinishedRaceNotifyAnalyticsDelegate;
    public UnityAction<string> OnPlayerFinishedRaceNotifyMapDelegate;
    public UnityAction<bool> OnPlayerFinishedRaceNotifyUIDelegate;

    Dictionary<string, Dictionary<string, System.TimeSpan>> playerTimes;

    public bool isMine = false;
    public Player player;

    public PhotonView GetPhotonView()
    {
        return photonView;
    }

    public override void Awake()
    {
        base.Awake();

        photonView = GetComponent<PhotonView>();

        // Destroy/disable stuff only needed for main player
        if (photonView.IsMine)
        {
            isMine = true;
            player = PhotonNetwork.LocalPlayer;
        }
        else
        {
            Destroy(GetComponent<AudioListener>());
        }

    }

    public void Start()
    {
        InitRaceData();
    }
    #endregion

    public void PlayerFinishedRaceEvent(int amountOfLaps, System.TimeSpan raceTime, double averageLapTime, string playerName)
    {
        if (photonView.IsMine)
        {
            // Camera
            bool success = playerCamera.ActivateSpectatorMode();

            // Analytics
            OnPlayerFinishedRaceNotifyAnalyticsDelegate(runData.maxLaps, runData.raceTime, averageLapTime, playerName);

            // UI
            OnPlayerFinishedRaceNotifyUIDelegate(success);
        }
        else
        {
            // Other ships don't have references to other client's camera
            GameObject myCameraObject = GameObject.FindGameObjectWithTag("MainCamera");
            PlayerCamera myCamera = myCameraObject.GetComponent<PlayerCamera>();

            myCamera.RemoveSpectatable(this);

            if (myCamera.SpectatablesLeft())
            {
                myCamera.NextSpectatable();
            }
            else
            {
                OnPlayerFinishedRaceNotifyUIDelegate(false);
            }
        }

        Destroy(GetComponent<PlayerController>());
        Destroy(GetComponent<ShipMovement>());
        transform.position = new Vector3(Random.Range(50000, 100000), Random.Range(50000, 100000), Random.Range(50000, 100000));
    }

    private void Update()
    {
        if (!runData.raceFinished && RaceManager.raceStarted)
            runData.raceTime = runData.raceTime.Add(System.TimeSpan.FromSeconds(1 * Time.deltaTime));

        // Save positions/rotations for replay, could also use coroutine
        if (Time.frameCount % 3 == 0)
        {
            runData.positionsX.Add(transform.position.x);
            runData.positionsY.Add(transform.position.y);
            runData.positionsZ.Add(transform.position.z);
            runData.replayRotations.Add(transform.rotation);
        }

        // Cheat finish
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.F))
        {
            CheatFinish();
        }
    }

    [ContextMenu("Finish")]
    public void CheatFinish()
    {
        runData.isOverHalfway = true;
        runData.currentLap = runData.maxLaps;
    }


    private void InitRaceData()
    {
        // Set currentlap, maxlaps, timer, pos
        runData.currentLap = 0;
            runData.maxLaps = 2;
        runData.raceTime = System.TimeSpan.Parse("00:00:00.000");
        runData.leaderboardTimes = new List<System.TimeSpan>();
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

    #region Calculate race position (rank) OFF
    //private int WAYPOINT_VALUE = 100;
    //private int LAP_VALUE = 10000;

    //public float GetDistance()
    //{
    //    if (runData.lastWaypoint != null)
    //        return (transform.position - runData.lastWaypoint.position).magnitude + runData.currentWaypoint * WAYPOINT_VALUE + runData.currentLap * LAP_VALUE;
    //    else return 1000000f;
    //}

    //public int GetShipPos(PlayerShip[] ships)
    //{
    //    float distance = GetDistance();
    //    //Debug.Log(distance);

    //    runData.currentPos = 1;
    //    foreach (PlayerShip ship in ships)
    //        if (ship.GetDistance() > distance)
    //            runData.currentPos++;
    //    return runData.currentPos;
    //}

    #endregion

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

            // Check for valid lap
            if (!runData.isWrongWay && runData.isOverHalfway)
            {
                // Save lap if race has started (lap > 0) // THIS ALWAYS HAPPENS REGARDLESS OF FINISHED OR NOT
                if (runData.currentLap > 0)
                {
                    // Update best time if not set (00:00:00)
                    // CheckBestTime()
                    if (runData.bestRaceTime == System.TimeSpan.Parse("00:00:00"))
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
                    if (SceneManager.GetActiveScene().name == ScenesInformation.sceneNames[SceneTitle.WASTELAND])
                    {
                        if (runData.raceTime < System.TimeSpan.Parse("00:02:15.000"))
                            AchievementManager.UpdateAchievement(0, 1f);
                        if (runData.raceTime < System.TimeSpan.Parse("00:02:05.000"))
                            AchievementManager.UpdateAchievement(1, 1f);
                        if (runData.raceTime < System.TimeSpan.Parse("00:02:00.000"))
                            AchievementManager.UpdateAchievement(2, 1f);
                    }
                    else if (SceneManager.GetActiveScene().name == ScenesInformation.sceneNames[SceneTitle.HIGHWAY])
                    {
                        if (runData.raceTime < System.TimeSpan.Parse("00:02:00.000"))
                            AchievementManager.UpdateAchievement(3, 1f);
                        if (runData.raceTime < System.TimeSpan.Parse("00:01:50.000"))
                            AchievementManager.UpdateAchievement(4, 1f);
                        if (runData.raceTime < System.TimeSpan.Parse("00:01:45.000"))
                            AchievementManager.UpdateAchievement(5, 1f);
                    }
                }

                // If finished
                if (runData.currentLap == runData.maxLaps) // 3/3 laps + finish
                {
                    Debug.Log("playerShip Finished");
                    levelSoundManager.PlaySound(SoundType.VICTORY);

                    // Leaderboard
                    Account account = Registration.GetCurrentAccount();
                    HighscoreManager.Instance.AddHighscoreEntry(account.username, runData.bestRaceTime.ToString(), SceneManager.GetActiveScene().name);

                    // Update bestTime for UI
                    if (!PlayerPrefs.HasKey("Highscore"))
                        PlayerPrefs.SetString("Highscore", runData.bestRaceTime.ToString());
                    else
                    {
                        if (System.TimeSpan.Parse(PlayerPrefs.GetString("Highscore")) > runData.bestRaceTime)
                            PlayerPrefs.SetString("Highscore", runData.bestRaceTime.ToString());
                    }

                    // Save replay
                    SaveReplay();

                    runData.raceFinished = true;

                    // Analytics
                    double averageLapTime = runData.raceTime.TotalSeconds / runData.maxLaps;
                    string playerName = "Anon";

                    if (PhotonNetwork.IsConnected)
                        playerName = PhotonNetwork.LocalPlayer.NickName;



                    PlayerFinishedRaceEvent(runData.maxLaps, runData.raceTime, averageLapTime, playerName);
                }
                else // If not finished
                {
                    // Reset Time. Only reset when lap > 0 && over half way
                    if (runData.currentLap > 0 && runData.isOverHalfway)
                    {
                        // Lap passed
                        if (runData.currentLap > 0)
                        {
                            Debug.Log($"PlayerShip Crossed Finish Line");
                            levelSoundManager.PlaySound(SoundType.LAPPASSED);

                            runData.isOverHalfway = false;

                            runData.currentLap++;
                        }
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
            runData.lastWaypoint = other.transform;
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
        Account account = Registration.GetCurrentAccount();
        ReplayManager.Instance.SaveReplay(account.username, SceneManager.GetActiveScene().name, runData.positionsX, runData.positionsY, runData.positionsZ, runData.replayRotations, runData.raceTime.ToString());
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