using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuSoundManager : SoundManager
{
    public AudioClip[] clickButtonClips;

    public override void PlaySound(SoundType soundType)
    {
        switch (soundType)
        {
            case SoundType.CLICKBUTTON:
                audioSource.clip = clickButtonClips[Random.Range(0, clickButtonClips.Length)];
                break;
        }

        if (!audioSource.isPlaying)
            audioSource.Play();
    }
}
