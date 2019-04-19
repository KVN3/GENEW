using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipSoundManager : SoundManager
{
    public AudioClip[] shutDownClips;
    public AudioClip[] restartClips;
    public AudioClip[] alarmClips;

    public AudioClip[] speedBoostClips;
    public AudioClip[] pickUpClips;

    public AudioClip[] shootingClips;
    public AudioClip[] protectedClips;

    public override void PlaySound(SoundType soundType)
    {
        switch (soundType)
        {
            case SoundType.SHUTDOWN:
                audioSource.clip = shutDownClips[Random.Range(0, shutDownClips.Length)];
                break;
            case SoundType.RESTART:
                audioSource.clip = restartClips[Random.Range(0, restartClips.Length)];
                break;
            case SoundType.ALARM:
                audioSource.clip = alarmClips[Random.Range(0, alarmClips.Length)];
                break;

            case SoundType.SPEEDBOOST:
                audioSource.clip = speedBoostClips[Random.Range(0, speedBoostClips.Length)];
                break;
            case SoundType.PICKUP:
                audioSource.clip = pickUpClips[Random.Range(0, pickUpClips.Length)];
                break;

            case SoundType.SHOOTING:
                audioSource.clip = shootingClips[Random.Range(0, shootingClips.Length)];
                break;
            case SoundType.PROTECTED:
                audioSource.clip = protectedClips[Random.Range(0, protectedClips.Length)];
                break;
        }

        audioSource.Play();
    }
}
