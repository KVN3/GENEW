using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Inherit this from a VoiceSoundManager and Sounds from a SFX manager which inherit the sound manager so you can disable Voice or sound
public class AISoundManager : SoundManager
{
    public AudioClip[] forcefieldAvailableClips;
    public AudioClip[] minesAvailableClips;
    public AudioClip[] projectilesAvailableClips;
    public AudioClip[] smokescreenAvailableClips;
    public AudioClip[] speedboostAvailableClips;

    public AudioClip[] hostilesDetectedClips;
    public AudioClip[] dangerDetectedClips;

    public AudioClip[] chargesDrainedClips;
    public AudioClip[] chargesRestoredClips;

    public AudioClip[] systemErrorClips;
    public AudioClip[] smokeDeployedClips;

    private bool hasAlreadyAlerted;
    private bool recentlyReportedOnCharges;
    private bool hasAlertedDanger;

    public bool active = true;

    public override void PlaySound(SoundType soundType)
    {
        // If inactive, don't continue
        if (!active)
            return;

        if (IsSoundMuted())
            return;

        // Select the right clip
        switch (soundType)
        {
            // Item pickups
            case SoundType.AIFORCEFIELD:
                audioSource.clip = forcefieldAvailableClips[Random.Range(0, forcefieldAvailableClips.Length)];
                break;
            case SoundType.AIMINES:
                audioSource.clip = minesAvailableClips[Random.Range(0, minesAvailableClips.Length)];
                break;
            case SoundType.AIPROJECTILES:
                audioSource.clip = projectilesAvailableClips[Random.Range(0, projectilesAvailableClips.Length)];
                break;
            case SoundType.AISMOKESCREEN:
                audioSource.clip = smokescreenAvailableClips[Random.Range(0, smokescreenAvailableClips.Length)];
                break;
            case SoundType.AISPEED:
                audioSource.clip = speedboostAvailableClips[Random.Range(0, speedboostAvailableClips.Length)];
                break;

            // Item used
            case SoundType.AISMOKEDEPLOYED:
                audioSource.clip = smokeDeployedClips[Random.Range(0, smokeDeployedClips.Length)];
                break;

            // Scans
            case SoundType.AIHOSTILESDETECTED:
                audioSource.clip = hostilesDetectedClips[Random.Range(0, hostilesDetectedClips.Length)];
                break;
            case SoundType.AIDANGER:
                audioSource.clip = dangerDetectedClips[Random.Range(0, dangerDetectedClips.Length)];
                break;

            // Charges
            case SoundType.AICHARGESDRAINED:
                audioSource.clip = chargesDrainedClips[Random.Range(0, chargesDrainedClips.Length)];
                break;
            case SoundType.AICHARGESRESTORED:
                audioSource.clip = chargesRestoredClips[Random.Range(0, chargesRestoredClips.Length)];
                break;

            // System
            case SoundType.AISYSTEMERROR:
                audioSource.clip = systemErrorClips[Random.Range(0, systemErrorClips.Length)];
                break;
        }

        // Play the sound, if not already playing one
        if (!audioSource.isPlaying)
            audioSource.Play();
    }

    #region Item
    public void PlayVoiceOnDelay(float seconds, Collectable item)
    {
        StartCoroutine(PlayAIVoice(item));
    }

    private IEnumerator PlayAIVoice(Collectable collectableItemClass)
    {
        yield return new WaitForSeconds(.5f);

        if (collectableItemClass is JammerProjectile)
            PlaySound(SoundType.AIPROJECTILES);

        else if (collectableItemClass is JammerMine)
            PlaySound(SoundType.AIMINES);

        else if (collectableItemClass is SmokeScreenItem)
            PlaySound(SoundType.AISMOKESCREEN);

        else if (collectableItemClass is ForcefieldItem)
            PlaySound(SoundType.AIFORCEFIELD);

        else if (collectableItemClass is SpeedBurst)
            PlaySound(SoundType.AISPEED);
    }
    #endregion

    public void PlayDangerSound(SoundType soundType)
    {
        if (Random.Range(0f, 1f) > .20f)
        {
            if (!hasAlertedDanger)
            {
                PlaySound(soundType);
                hasAlertedDanger = true;
            }
        }
    }

    #region Charges & System
    public void ReportSystemError(SoundType soundType)
    {
        PlaySound(soundType);
        StartCoroutine(SetChargesReportCooldown(5f));
    }

    public void TryToReportCharges(float charges, SoundType soundType)
    {
        if (!recentlyReportedOnCharges)
        {
            PlaySound(soundType);
            StartCoroutine(SetChargesReportCooldown(10f));
        }

    }

    private IEnumerator SetChargesReportCooldown(float seconds)
    {
        recentlyReportedOnCharges = true;
        yield return new WaitForSeconds(seconds);
        recentlyReportedOnCharges = false;
    }
    #endregion


    public bool HasAlreadyAlerted()
    {
        return hasAlreadyAlerted;
    }

    public void SetAlerted()
    {
        hasAlreadyAlerted = true;
    }
}
