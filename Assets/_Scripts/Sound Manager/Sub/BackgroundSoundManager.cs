using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundSoundManager : SoundManager
{
    public AudioClip[] randomClips;

    public void Start()
    {
        StartCoroutine(RandomSoundLoop());
    }

    private IEnumerator RandomSoundLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(10, 20));

            if(!audioSource.isPlaying)
            {
                if (Random.Range(0, 3) == 0)
                {
                    audioSource.clip = randomClips[Random.Range(0, randomClips.Length)];
                    audioSource.Play();
                }
            }
        }
        
    }
}