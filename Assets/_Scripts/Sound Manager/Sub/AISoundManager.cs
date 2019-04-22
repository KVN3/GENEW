using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AISoundManager : SoundManager
{
    public AudioClip[] forcefieldAvailableClips;
    public AudioClip[] minesAvailableClips;
    public AudioClip[] projectilesAvailableClips;
    public AudioClip[] smokescreenAvailableClips;
    public AudioClip[] speedboostAvailableClips;

    public AudioClip[] hostilesDetectedAudioClips;

    private bool hasAlreadyAlerted;

    public override void PlaySound(SoundType soundType)
    {
        switch (soundType)
        {
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

            case SoundType.AIHOSTILESDETECTED:
                audioSource.clip = hostilesDetectedAudioClips[Random.Range(0, hostilesDetectedAudioClips.Length)];
                break;
        }

        if (!audioSource.isPlaying)
            audioSource.Play();
    }

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

    public bool HasAlreadyAlerted()
    {
        return hasAlreadyAlerted;
    }

    public void SetAlerted()
    {
        hasAlreadyAlerted = true;
    }
}
