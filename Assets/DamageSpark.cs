using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageSpark : MonoBehaviour
{
    private ParticleSystem ps;
    private ParticleSystem.MainModule main;

    public void Start()
    {
        ps = GetComponent<ParticleSystem>();
        main = ps.main;
    }

    public void Activate()
    {
        if (!ps.isPlaying)
        {
            ps.Play();
        }
    }

    public void Deactivate()
    {
        if (ps.isPlaying)
        {
            ps.Stop();
        }
    }
}
