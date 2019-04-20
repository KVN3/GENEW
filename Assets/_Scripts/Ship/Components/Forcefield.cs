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

    public void Awake()
    {
        charges = initialCharges;
        gameObject.SetActive(false);
    }

    public bool IsActivate()
    {
        return gameObject.activeInHierarchy;
    }

    public void Activated(bool manualActivate)
    {
        gameObject.SetActive(true);

        if(manualActivate)
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
            charges = maxCharges;
    }

    private void DrainCharges(float drain)
    {
        if (charges > 0)
            charges -= drain;
        else if (charges < 1)
        {
            charges = 0;
            StartCoroutine(RegenCooldown());
        }
    }

    public void GetHit(float dmg)
    {
        charges -= dmg;
    }

    private IEnumerator RegenCooldown()
    {
        hasRegenerationCooldown = true;
        yield return new WaitForSeconds(3);
        hasRegenerationCooldown = false;
    }

    #region ForcefieldItem
    public void ActivateBoostedForcefield(int duration)
    {
        StartCoroutine(BoostedForcefield(duration));
    }

    private IEnumerator BoostedForcefield(int duration)
    {
        itemActive = true;
        gameObject.SetActive(true);

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
        if (charges > 10)
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
