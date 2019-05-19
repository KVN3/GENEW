using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LevelSoundManager : SoundManager
{
    public AudioClip[] victoryClips;
    public AudioClip[] lossClips;
    public AudioClip[] lapPassedClips;
    public AudioClip[] achievementClips;

    public bool active = true;
    public override void PlaySound(SoundType soundType)
    {
        // If not active, then don't continue
        if (!active)
            return;

        if (IsSoundMuted())
            return;

        // Get the sound type clip
        switch (soundType)
        {
            case SoundType.VICTORY:
                audioSource.clip = victoryClips[Random.Range(0, victoryClips.Length)];
                break;
            case SoundType.LOSS:
                audioSource.clip = lossClips[Random.Range(0, lossClips.Length)];
                break;
            case SoundType.LAPPASSED:
                audioSource.clip = lapPassedClips[Random.Range(0, lapPassedClips.Length)];
                break;
            case SoundType.ACHIEVEMENT:
                audioSource.clip = achievementClips[Random.Range(0, achievementClips.Length)];
                break;
        }

        // Play the sound
        audioSource.Play();
    }
}
