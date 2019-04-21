using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public ShipSoundManager shipSoundManagerClass;
    public LevelSoundManager levelSoundManagerClass;
    public DamageSpark spark;

    private List<ShipComponent> componentsList;
    private ShipSoundManager shipSoundManager;
    private LevelSoundManager levelSoundManager;

    // Collectables
    private Collectable collectableItemClass;
    private int itemAmount;

    private bool recentlyHit;

    private PhotonView photonView;

    public virtual void Awake()
    {
        shipSoundManager = Instantiate(shipSoundManagerClass, transform.localPosition, transform.localRotation, this.transform);

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
                    //photonView.RPC("Fire", RpcTarget.AllViaServer);

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
                SmokeScreenItem smokeScreenItem = (SmokeScreenItem)collectableItemClass;
                Instantiate(smokeScreenItem, transform.position, Quaternion.identity);
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
        }
    }

    void OnCollisionEnter(Collision other)
    {
        if (!components.forcefield.IsActivate())
        {
            if (!other.gameObject.CompareTag("EnergyBall"))
            {
                if (!other.gameObject.CompareTag("ShipComponent"))
                {
                    GetHitByRegular();
                }
            }
        }
    }

    public void GetHitByRegular()
    {
        shipSoundManager.PlaySound(SoundType.ALARM);
        spark.Activate();
    }

    public void GetHitByEmp(int duration)
    {
        if (!components.system.IsSystemDown())
        {
            if (!components.forcefield.IsActivate())
            {
                components.system.ShutDown();
                components.engines.RestoreSystem();

                StartCoroutine(GotHit());
            }
            else
            {
                components.forcefield.GetHit(30);
                shipSoundManager.PlaySound(SoundType.PROTECTED);
            }
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

    public void SetItem(Collectable item, int amount)
    {
        this.collectableItemClass = item;
        itemAmount = amount;
    }

    public int GetItemAmount()
    {
        return itemAmount;
    }

    public Collectable GetItem()
    {
        return collectableItemClass;
    }

    public bool WasRecentlyHit()
    {
        if (recentlyHit)
            return true;
        return false;
    }


    public ShipSoundManager GetShipSoundManager()
    {
        return shipSoundManager;
    }
}

// TO DO: camera straight while turning
// TO DO: temporary increase of air drag while turning (big question mark)