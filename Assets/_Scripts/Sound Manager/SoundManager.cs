using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    protected AudioSource audioSource;
    protected AudioClip audioClip;

    public virtual void Awake()
    {
        InitializeComponent();
    }

    public void InitializeComponent()
    {
        audioSource = this.GetComponent<AudioSource>();
    }

    // Ideally IsSoundMuted would be here and can stop the overridden method
    public virtual void PlaySound(SoundType soundType)
    {
       
    }

    public bool IsSoundMuted()
    {
        if (GameConfiguration.SoundOn)
            return false;
        else return true;
    }
}
