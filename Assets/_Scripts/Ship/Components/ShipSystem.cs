using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ShipSystem : ShipComponent
{
    [SerializeField]
    private float flashDuration = 0.025f;

    private bool systemsDown;
    private bool recentlyHit;

    public UnityAction<int, string, bool> OnPlayerStunnedDelegate;
    public UnityAction<float, FlashColor> OnPlayerShipHitDelegate;


    // Shut down ship / crash
    public void ShutDown()
    {
        shipSoundManager.PlaySound(SoundType.SHUTDOWN);
        systemsDown = true;

        Rigidbody rb = parentShip.GetComponent<Rigidbody>();
        rb.useGravity = true;

        parentShip.components.engines.TurnOffAllEngines();
    }

    // Boot up ship after a crash
    public void StartUp()
    {
        shipSoundManager.PlaySound(SoundType.RESTART);
        systemsDown = false;

        Rigidbody rb = parentShip.GetComponent<Rigidbody>();
        rb.useGravity = false;
    }

    // Called when a ship gets hit by an object that can stun the ship. Stuns the ship if not protected.
    public void TryToStun(int duration, string cause)
    {
        // System not down
        if (!IsSystemDown())
        {
            bool playerWasProtected = false;

            // Forcefield not active, shutdown
            if (!parentShip.components.forcefield.IsActive())
            {
                // Tutorial script update
                if (ScenesInformation.IsTutorialScene())
                    TutorialManager.Instance.ShipStunned = true;

                // Screen flash & AI error
                OnPlayerShipHitDelegate(flashDuration, FlashColor.RED);
                parentShip.GetAiSoundManager().ReportSystemError(SoundType.AISYSTEMERROR);

                // Shutdown and start restore system procedure
                parentShip.components.system.ShutDown();
                parentShip.components.engines.RestoreSystem();

                // Set get hit cooldown and free forcefield
                StartCoroutine(C_GotHit());
            }

            // Forcefield active, take damage
            else
            {
                OnPlayerShipHitDelegate(flashDuration, FlashColor.BLUE);

                // Achievement
                parentShip.blockedProjectiles++;
                AchievementManager.UpdateAchievement(12, 1f);

                // Forcefield takes damage and plays protected sound
                parentShip.components.forcefield.GetHit(30);
                shipSoundManager.PlaySound(SoundType.PROTECTED);
                playerWasProtected = true;
            }

            // Delegate event
            if (GetComponent<PhotonView>().IsMine)
            {
                OnPlayerStunnedDelegate(duration, cause, playerWasProtected);
            }

        }
    }

    // Get hit and activate free forcefield and hit cooldown
    private IEnumerator C_GotHit()
    {
        recentlyHit = true;
        parentShip.gotHit = true;
        yield return new WaitForSeconds(1);

        // Temp free forcefield
        parentShip.components.forcefield.Activated(false);
        yield return new WaitForSeconds(4);
        recentlyHit = false;
    }

    // Shut down for duration given
    public IEnumerator TemporaryShutDown(int duration)
    {
        if (!IsSystemDown())
        {
            ShutDown();
            yield return new WaitForSeconds(duration);
            parentShip.components.engines.RestoreSystem();
        }
    }

    public void FlashScreen(FlashColor color)
    {
        OnPlayerShipHitDelegate(flashDuration, color);
    }

    #region GetSet
    public bool IsSystemDown()
    {
        return systemsDown;
    }

    public bool WasRecentlyHit()
    {
        if (recentlyHit)
            return true;
        return false;
    }
    #endregion
}
