using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipForcefield : ShipComponent, IPunObservable
{
    public float maxCharges;
    public float drainRate;
    public float regainRate;
    public float initialCharges;

    private float charges;
    private bool hasRegenerationCooldown;
    private bool itemActive;

    private AudioSource audioSource;

    public PlayerShip owner;

    [SerializeField]
    private GameObject forcefieldObject;

    private PhotonView photonView;

    public void Awake()
    {
        charges = initialCharges;
        forcefieldObject.SetActive(false);

        photonView = GetComponent<PhotonView>();
        audioSource = forcefieldObject.GetComponent<AudioSource>();
    }

    #region Photon
    private bool forcefieldActive = false;

    // Send data if this is our ship, receive data if it is not
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(forcefieldActive);
        }
        else
        {
            forcefieldActive = (bool)stream.ReceiveNext();

            if (forcefieldActive)
                forcefieldObject.SetActive(true);
            else
                forcefieldObject.SetActive(false);
        }
    }
    #endregion

    public bool IsActive()
    {
        return forcefieldObject.activeInHierarchy;
    }

    public void Activated(bool manualActivate)
    {
        if (!photonView.IsMine)
            return;

        if (ClientConfigurationManager.Instance.clientConfiguration.SoundOn)
            audioSource.enabled = true;
        else
            audioSource.enabled = false;

        forcefieldObject.SetActive(true);
        forcefieldActive = true;

        if (manualActivate)
            DrainCharges(drainRate);
        else
            DrainCharges(100);
    }

    public void Deactivated()
    {
        if (!photonView.IsMine)
            return;

        if (ClientConfigurationManager.Instance.clientConfiguration.SoundOn)
            audioSource.enabled = true;
        else
            audioSource.enabled = false;

        forcefieldObject.SetActive(false);
        forcefieldActive = false;

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
        if (photonView.IsMine)
        {
            forcefieldObject.SetActive(false);
            forcefieldActive = false;

            hasRegenerationCooldown = true;
            yield return new WaitForSeconds(3);
            hasRegenerationCooldown = false;
        }
    }

    #region ForcefieldItem
    public void ActivateBoostedForcefield(int duration)
    {
        if (!photonView.IsMine)
            return;

        forcefieldObject.SetActive(true);
        forcefieldActive = true;

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
