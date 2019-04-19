using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySoundManager : SoundManager
{
    public AudioClip[] shutDownClips;
    public AudioClip[] restartClips;
    public AudioClip[] alarmClips;

    public AudioClip[] speedBoostClips;
    public AudioClip[] pickUpClips;

    public AudioClip[] shootingClips;

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

            case SoundType.SHOOTING:
                audioSource.clip = shootingClips[Random.Range(0, shootingClips.Length)];
                break;
        }

        audioSource.Play();
    }
}
