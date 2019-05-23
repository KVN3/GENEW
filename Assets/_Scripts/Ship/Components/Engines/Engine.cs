using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Engine : MonoBehaviour
{
    public ParticleSystem particleSystem;

    private AudioSource audioSource;

    private Color originalColor;
    private float originalSpeed;
    private float originalLifetime;

    private ParticleSystem.MainModule pMain;

    private bool engineOn;

    // Start is called before the first frame update
    void Start()
    {
        particleSystem = GetComponent<ParticleSystem>();
        audioSource = GetComponent<AudioSource>();
        audioSource.enabled = false;

        Deactivate();

        pMain = particleSystem.main;
        originalColor = pMain.startColor.color;
        originalSpeed = pMain.startSpeed.constant;
        originalLifetime = pMain.startLifetime.constant;
    }

    public void FixedUpdate()
    {

    }

    public void Activate()
    {
        if (!particleSystem.isPlaying)
        {
            particleSystem.Play();
            engineOn = true;


            if (audioSource != null)
            {
                if (GameConfiguration.SoundOn)
                    audioSource.enabled = true;
                else
                    audioSource.enabled = false;
            }
        }

    }

    public void Deactivate()
    {
        if (particleSystem.isPlaying)
        {
            particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            engineOn = false;

            if (audioSource != null)
                audioSource.enabled = false;
        }
    }

    public void SetStartSpeed(float speed)
    {
        pMain.startSpeed = speed;
    }

    public void SetLifeTime(float lifeTime)
    {
        pMain.startLifetime = lifeTime;
    }

    public void SetBoostColor()
    {
        pMain.startColor = new Color(0, 191, 0, 255);
        //StartCoroutine(ClearOldParticles());

    }

    public void RestoreColor()
    {
        pMain.startColor = originalColor;
    }

    private IEnumerator ClearOldParticles()
    {
        SetStartSpeed(10);
        yield return new WaitForSeconds(1);
        SetStartSpeed(originalSpeed);
    }
}
