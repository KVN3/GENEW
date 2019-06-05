using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance;

    public AudioSource audioSource;

    public AudioClip[] instructionAudioClips;
    public string[] instructionStrings;

    public GameObject[] invisibleWalls;

    public bool checkpointReached = true;

    private int wallIndex = 0;

    public bool BoostActivated = false;
    public bool RocketPickedUp = false;
    public bool ShipStunned = false;
    public bool EnemyKilled = false;

    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;

        // PhotonNetwork.OfflineMode = true;
        audioSource = GetComponent<AudioSource>();

        // 0 "Welcome, racer. I've been tasked to guide you through your beginning steps."
        //  1 Let us begin with controls. The "W", "A", "S", "D", and the arrow keys, can be used to move the ship. Try this now.
        //  2 For your utility, I've provided you with an overview of all available controls in your dashboard.

        // 3 The green arrows on the ground indicate a boost pad. Move over them for an increase in velocity.
        // 4 Good. Now move over the next pad to receive an item.

        // 5 You're equipped with a rocket now. Try shooting that ship ahead of you by pressing "E" to see what happens.

        // 6 Your ship is equipped with a forcefield. To activate yours, hold down the "SPACE" key. Keep an eye on the charge bar just below the speed meter, you wouldn't want to run out of charges in the middle of a cloud of toxic gas.
        // 7 During your races you will encounter many different hostile energy spheres. Most of them hostile. Contact will likely bring your ship to a temporary shutdown.
        // 8 Try using the ship's forcefield to destroy that energy sphere ahead of you. Keep in mind though. Your forcefield is not infinite, it can overload.
        // 9This was a most, easy opponent. If you're ever being hunted by a pack of chasers, remember you can look back by holding down the "F" key. This will allow you to time the use of your forcefield, as well as deploy mines and dodge much more accurately.
        // 10 Mines carry a chip that detects friend from foe. Consider dropping them on valuable pads, to reserve them for yourself. Deploy a smoke screen in an area with many hostiles or activate your forcefield before shooting a wall nearby, to catch a follow-up ship off guard.


    }

    void Start()
    {
        //PlayerShip ps = playerShip.GetComponent<PlayerShip>();

        //PlayerCamera camera = playerCamera.GetComponent<PlayerCamera>();

        //PlayerController pc = playerShip.GetComponent<PlayerController>();
        //pc.SetPlayerCamera(camera);

        StartCoroutine(C_RelayInstructions());
    }

    public IEnumerator C_RelayInstructions()
    {
        while(InstructionText.Instance == null)
        {
            yield return new WaitForSeconds(.5f);
        }

        int i = 0;

        while (i != instructionAudioClips.Length)
        {
            if (ReadyToPlayNextInstruction(i))
            {
                // Speed wall
                if (i == 3)
                    TryRemovingWall(0);

                // Stun wall
                if (i == 7)
                    TryRemovingWall(1);

                // Enemy wall
                if (i == 9)
                    TryRemovingWall(2);

                PlayInstruction(i);

                i++;
            }
            else
            {
                print("Can't advange logs yet");
            }


            yield return new WaitForSeconds(1);
        }
    }

    private void PlayInstruction(int index)
    {
        audioSource.clip = instructionAudioClips[index];
        InstructionText.Instance.SetText(instructionStrings[index]);
        audioSource.Play();
    }

    private void TryRemovingWall(int index)
    {
        if (invisibleWalls[index].activeInHierarchy)
        {
            invisibleWalls[index].SetActive(false);
            wallIndex++;
        }
    }


    private bool ReadyToPlayNextInstruction(int index)
    {
        if (audioSource.isPlaying)
            return false;

        // Good. Now move over the next pad to receive an item. LOWER WALL AFTER USING BOOST PAD
        if (index == 4)
        {
            if (!BoostActivated)
            {
                return false;
            }
        }

        if (index == 5)
        {
            if (!RocketPickedUp)
            {
                return false;
            }
        }

        // After stunning ship
        if (index == 7)
        {
            if (!ShipStunned)
            {
                return false;
            }
        }

        // After killing enemy
        if (index == 9)
        {
            if (!EnemyKilled)
            {
                return false;
            }
        }

        // Complete tutorial achievement
        if (index == 10)
            AchievementManager.UpdateAchievement(0, 1f);

        return true;
    }
}