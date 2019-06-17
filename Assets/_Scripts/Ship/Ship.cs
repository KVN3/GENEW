using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.Assertions;
using UnityEngine.Events;

[System.Serializable]
public struct ShipComponents
{
    [Tooltip("Ship movement component.")]
    public ShipMovement movement;

    [Tooltip("Ship engines component.")]
    public ShipEngines engines;

    [Tooltip("Ship gun/weapon component.")]
    public ShipGun gun;

    [Tooltip("System ship component.")]
    public ShipSystem system;

    [Tooltip("Forcefield ship component.")]
    public ShipForcefield forcefield;
}

[RequireComponent(typeof(Rigidbody))]
public class Ship : MonoBehaviour
{
    #region Assigned variables
    // Ship component scripts that manage the various ship functions
    public ShipComponents components;

    // Sound manager classes
    [SerializeField]
    private ShipSoundManager shipSoundManagerClass;
    [SerializeField]
    protected LevelSoundManager levelSoundManagerClass;
    [SerializeField]
    protected AISoundManager aiSoundManagerClass;

    // Spark particle system
    [SerializeField]
    private DamageSpark spark;
    #endregion

    // Sound manager instances
    protected ShipSoundManager shipSoundManager;
    protected LevelSoundManager levelSoundManager;
    protected AISoundManager aiSoundManager;

    // Item run data
    public Collectable collectableItemClass;
    public int itemAmount;

    // Run time
    protected PhotonView photonView;

    // Achievement stats
    public bool gotHit = false;
    public bool usedBoost;
    protected bool usedItem;
    public int boostUses = 0;
    public int itemUses = 0;
    public int totalBoostUses;
    public int totalItemUses;
    public int blockedProjectiles = 0;
    public int totalBlockedProjectiles;

    // Delegates
    public UnityAction<Collectable, int> OnItemUsedDelegate;



    public virtual void Awake()
    {
        Assert.IsNotNull(shipSoundManagerClass);
        Assert.IsNotNull(aiSoundManagerClass);
        Assert.IsNotNull(levelSoundManagerClass);
        Assert.IsNotNull(components.system);
        Assert.IsNotNull(components.gun);
        Assert.IsNotNull(components.movement);
        Assert.IsNotNull(components.forcefield);
        Assert.IsNotNull(components.engines);
        Assert.IsNotNull(spark);

        // Instantiate Sound Managers
        shipSoundManager = Instantiate(shipSoundManagerClass, transform.localPosition, transform.localRotation, this.transform);
        aiSoundManager = Instantiate(aiSoundManagerClass, transform.localPosition, transform.localRotation, this.transform);
        levelSoundManager = Instantiate(levelSoundManagerClass, transform.localPosition, transform.localRotation, this.transform);

        // Get Components
        photonView = GetComponent<PhotonView>();

        // Disable these sound managers if this isn't this client's ship
        if (!photonView.IsMine)
        {
            aiSoundManager.active = false;
            levelSoundManager.active = false;
        }

        // Assign data to these components
        List<ShipComponent> componentsList = new List<ShipComponent>();
        componentsList.Add(components.movement);
        componentsList.Add(components.engines);
        componentsList.Add(components.gun);
        componentsList.Add(components.system);

        foreach (ShipComponent component in componentsList)
        {
            component.SetParentShip(this);
            component.SetShipSoundManager(shipSoundManager);
        }
    }

    // Use currently equipped item
    public void UseItem()
    {
        if (itemAmount > 0)
        {
            // Achievment data
            usedItem = true;
            itemUses++;
            totalItemUses++;
            AchievementManager.UpdateAchievement(18, 1f);

            if (collectableItemClass is JammerProjectile)
            {
                if (!components.gun.OnCooldown())
                {
                    components.gun.Shoot((JammerProjectile)collectableItemClass);
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

            // Send item used event
            OnItemUsedDelegate(collectableItemClass, itemAmount);
        }
    }

    protected void OnCollisionEnter(Collision other)
    {
        if (components.forcefield.IsActive())
        {
            // Flash screen blue
            components.system.FlashScreen(FlashColor.BLUE);
        }
        else
        {
            // Register this ship getting hit by anything but a mine and energyball. Owned projectiles do not register hits.
            if (other.gameObject.CompareTag("Projectile"))
            {
                Ship ownerShip = other.gameObject.GetComponent<JammerProjectile>().owner;
                if (ownerShip != this)
                    GetHitByRegular(other);
            }
            else if (!other.gameObject.CompareTag("ShipComponent") && !other.gameObject.CompareTag("Mine") && !other.gameObject.CompareTag("EnergyBall"))
                GetHitByRegular(other);
        }
    }

    #region GETTING HIT
    // Ship got hit by regular, e.a. a wall
    public void GetHitByRegular(Collision other)
    {
        components.system.FlashScreen(FlashColor.RED);

        // Sound & effect
        shipSoundManager.PlaySound(SoundType.ALARM);
        spark.Activate();

        // Lower ship health if master client
        if (PhotonNetwork.IsMasterClient)
            PlayerManager.Instance.ModifyHealth(photonView.Owner, -5);
    }
    #endregion

    #region UNUSED
    

    public void Alert()
    {
        if (!aiSoundManager.HasAlreadyAlerted())
        {
            aiSoundManager.PlaySound(SoundType.AIHOSTILESDETECTED);
            aiSoundManager.SetAlerted();
        }
    }
    #endregion

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

    // Sound managers
    public ShipSoundManager GetShipSoundManager()
    {
        return shipSoundManager;
    }

    public AISoundManager GetAiSoundManager()
    {
        return aiSoundManager;
    }
    #endregion
}

