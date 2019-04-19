using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundSoundManager : SoundManager
{
    public AudioClip[] randomClips;

    public void FixedUpdate()
    {
        if (!audioSource.isPlaying)
        {
            audioSource.clip = randomClips[Random.Range(0, randomClips.Length)];
            audioSource.Play();
        }
    }
}
