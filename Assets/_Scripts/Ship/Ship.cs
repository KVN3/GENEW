using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.Events;

[System.Serializable]
public struct ShipComponents
{
    public ShipMovement movement;
    public ShipEngines engines;
    public ShipGun gun;
    public ShipSystem system;
    public Forcefield forcefield;
}

public class Ship : MyMonoBehaviour
{
    public ShipComponents components;

    #region ObjectClasses
    [SerializeField]
    private ShipSoundManager shipSoundManagerClass;

    [SerializeField]
    private LevelSoundManager levelSoundManagerClass;

    [SerializeField]
    private AISoundManager aiSoundManagerClass;

    [SerializeField]
    private DamageSpark spark;
    #endregion

    private List<ShipComponent> componentsList;

    // Instantiated
    protected ShipSoundManager shipSoundManager;
    protected LevelSoundManager levelSoundManager;
    protected AISoundManager aiSoundManager;

    // Collectables
    public Collectable collectableItemClass;
    public int itemAmount;

    private bool recentlyHit;

    private PhotonView photonView;

    public UnityAction<Collectable, int> OnItemUsedDelegate;
    public UnityAction<int, string, bool> OnPlayerStunnedDelegate;

    public virtual void Awake()
    {
        shipSoundManager = Instantiate(shipSoundManagerClass, transform.localPosition, transform.localRotation, this.transform);
        aiSoundManager = Instantiate(aiSoundManagerClass, transform.localPosition, transform.localRotation, this.transform);
        levelSoundManager = Instantiate(levelSoundManagerClass, transform.localPosition, transform.localRotation, this.transform);

        componentsList = new List<ShipComponent>();

        componentsList.Add(components.movement);
        componentsList.Add(components.engines);
        componentsList.Add(components.gun);
        componentsList.Add(components.system);

        foreach (ShipComponent component in componentsList)
        {
            component.SetParentShip(this);
            component.SetShipSoundManager(shipSoundManager);
        }

        photonView = GetComponent<PhotonView>();
    }

    public virtual void Start()
    {

    }

    public void UseItem()
    {
        if (itemAmount > 0)
        {
            if (collectableItemClass is JammerProjectile)
            {
                if (!components.gun.OnCooldown())
                {
                    components.gun.Shoot((JammerProjectile)collectableItemClass);
                    //photonView.RPC("Fire", RpcTarget.AllViaServer);
                    itemAmount--;
                }
            }
            else if (collectableItemClass is JammerMine)
            {
                if (!components.gun.OnCooldown())
                {
                    components.gun.DropMine((JammerMine)collectableItemClass);
                    itemAmount--;
                }
            }
            else if (collectableItemClass is SmokeScreenItem)
            {
                SmokeScreenItem smokeScreenItem = (SmokeScreenItem)collectableItemClass;

                aiSoundManager.PlaySound(SoundType.AISMOKEDEPLOYED);
                components.gun.DropSmokeScreen((SmokeScreenItem)collectableItemClass);

                itemAmount--;
            }
            else if (collectableItemClass is ForcefieldItem)
            {
                ForcefieldItem forcefieldItem = (ForcefieldItem)collectableItemClass;
                components.forcefield.ActivateBoostedForcefield(forcefieldItem.duration);
                itemAmount--;
            }
            else if (collectableItemClass is SpeedBurst)
            {
                SpeedBurst speedBurstItem = (SpeedBurst)collectableItemClass;
                components.movement.ActivateSpeedBoost(speedBurstItem.maxSpeedIncrease, speedBurstItem.boostFactor, speedBurstItem.boostDuration);
                itemAmount--;
            }

            // Delegate event
            OnItemUsedDelegate(collectableItemClass, itemAmount);
        }
    }


    new void OnCollisionEnter(Collision other)
    {
        if (!components.forcefield.IsActive())
        {
            if (other.gameObject.CompareTag("Projectile"))
            {
                Ship ownerShip = other.gameObject.GetComponent<JammerProjectile>().owner;
                if (!ownerShip == this)
                    GetHitByRegular();
            }
            else if (!other.gameObject.CompareTag("ShipComponent") && !other.gameObject.CompareTag("Mine") && !other.gameObject.CompareTag("EnergyBall"))
                GetHitByRegular();
        }
    }

    // Ship got hit by regular, e.a. a wall
    public void GetHitByRegular()
    {
        shipSoundManager.PlaySound(SoundType.ALARM);
        spark.Activate();
    }

    // Ship got hit by a stun
    public void GetHitByEmp(int duration, string cause)
    {
        // System not down
        if (!components.system.IsSystemDown())
        {
            bool playerWasProtected = false;

            // Forcefield not active, shutdown
            if (!components.forcefield.IsActive())
            {
                aiSoundManager.ReportSystemError(SoundType.AISYSTEMERROR);

                components.system.ShutDown();
                components.engines.RestoreSystem();

                StartCoroutine(GotHit());
            }

            // Forcefield active, take damage
            else
            {
                components.forcefield.GetHit(30);
                shipSoundManager.PlaySound(SoundType.PROTECTED);

                playerWasProtected = true;
            }

            // Delegate event
            OnPlayerStunnedDelegate(duration, cause, playerWasProtected);
        }
    }

    private IEnumerator GotHit()
    {
        recentlyHit = true;
        yield return new WaitForSeconds(1);

        // Temp free forcefield
        components.forcefield.Activated(false);
        yield return new WaitForSeconds(4);
        recentlyHit = false;
    }



    public void Alert()
    {
        if (!aiSoundManager.HasAlreadyAlerted())
        {
            aiSoundManager.PlaySound(SoundType.AIHOSTILESDETECTED);
            aiSoundManager.SetAlerted();
        }
    }

    #region GetSet
    // Collectable Items
    public void SetItem(Collectable item, int amount)
    {
        this.collectableItemClass = item;
        itemAmount = amount;

        aiSoundManager.PlayVoiceOnDelay(0.5f, item);
    }

    public Collectable GetItem()
    {
        return collectableItemClass;
    }

    public int GetItemAmount()
    {
        return itemAmount;
    }

    // Hit
    public bool WasRecentlyHit()
    {
        if (recentlyHit)
            return true;
        return false;
    }

    // Sound managers
    public ShipSoundManager GetShipSoundManager()
    {
        return shipSoundManager;
    }

    public AISoundManager GetAiSoundManager()
    {
        return aiSoundManager;
    }

    // Photon
    public PhotonView GetPhotonView()
    {
        return photonView;
    }
    #endregion
}

// TO DO: camera straight while turning
// TO DO: temporary increase of air drag while turning (big question mark)