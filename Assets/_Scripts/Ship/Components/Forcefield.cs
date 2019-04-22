using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Forcefield : ShipComponent
{
    public float maxCharges;
    public float drainRate;
    public float regainRate;
    public float initialCharges;

    private float charges;
    private bool hasRegenerationCooldown;
    private bool itemActive;

    public PlayerShip owner;

    public void Awake()
    {
        charges = initialCharges;
        gameObject.SetActive(false);
    }

    public void Start()
    {

    }

    public bool IsActivate()
    {
        return gameObject.activeInHierarchy;
    }

    public void Activated(bool manualActivate)
    {
        gameObject.SetActive(true);

        if (manualActivate)
            DrainCharges(drainRate);
        else
            DrainCharges(100);
    }

    public void Deactivated()
    {
        gameObject.SetActive(false);
        RegainCharges();
    }

    private void RegainCharges()
    {
        if (charges < maxCharges)
            charges += regainRate;
        else if (charges > maxCharges)
        {
            charges = maxCharges;
            owner.GetAiSoundManager().TryToReportCharges(charges, SoundType.AICHARGESRESTORED);
        }

    }

    private void DrainCharges(float drain)
    {
        charges -= drain;
        if (charges < 5)
        {
            owner.GetAiSoundManager().TryToReportCharges(charges, SoundType.AICHARGESDRAINED);
            charges = -10;
        }

    }

    public void GetHit(float dmg)
    {
        charges -= dmg;
    }

    private IEnumerator RegenCooldown()
    {
        gameObject.SetActive(false);
        hasRegenerationCooldown = true;
        yield return new WaitForSeconds(3);
        hasRegenerationCooldown = false;
    }

    #region ForcefieldItem
    public void ActivateBoostedForcefield(int duration)
    {
        gameObject.SetActive(true);
        StartCoroutine(BoostedForcefield(duration));
    }

    private IEnumerator BoostedForcefield(int duration)
    {
        itemActive = true;

        yield return new WaitForSeconds(duration);

        itemActive = false;
    }

    public bool IsItemActive()
    {
        return itemActive;
    }
    #endregion


    public float GetCharges()
    {
        return charges;
    }

    public bool HasEnoughCharges()
    {
        if (charges > 0)
            return true;

        return false;
    }

    public bool IsOnCooldown()
    {
        if (hasRegenerationCooldown)
            return true;

        return false;
    }
}
