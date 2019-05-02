﻿using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class RandomMover : EnergyBall
{
    public float velocidadMax;

    public Shooter shooterModule;

    public float xMax;
    public float zMax;
    public float xMin;
    public float zMin;
    public float maxSpeed;

    private float x;
    private float z;
    private float tmp;
    private float angulo;
    private Rigidbody rb;

    // Use this for initialization
    public override void Start()
    {
        base.Start();

        shooterModule.SetSoundManager(enemySoundManager);

        x = Random.Range(-velocidadMax, velocidadMax);
        z = Random.Range(-velocidadMax, velocidadMax);
        angulo = Mathf.Atan2(x, z) * (180 / 3.141592f) + 90;
        transform.localRotation = Quaternion.Euler(0, angulo, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Move();
        }
        else
        {
            SmoothMove();
        }
    }

    private void Move()
    {
        rb = GetComponent<Rigidbody>();
        tmp += Time.deltaTime;

        if (transform.localPosition.x > xMax)
        {
            x = Random.Range(-velocidadMax, 0.0f);
            tmp = 0.0f;
        }
        if (transform.localPosition.x < xMin)
        {
            x = Random.Range(0.0f, velocidadMax);
            tmp = 0.0f;
        }
        if (transform.localPosition.z > zMax)
        {
            z = Random.Range(-velocidadMax, 0.0f);
            tmp = 0.0f;
        }
        if (transform.localPosition.z < zMin)
        {
            z = Random.Range(0.0f, velocidadMax);
            tmp = 0.0f;
        }

        if (tmp > 1.0f)
        {
            x = Random.Range(-velocidadMax, velocidadMax);
            z = Random.Range(-velocidadMax, velocidadMax);
            tmp = 0.0f;
        }

        if (rb.velocity.x > maxSpeed)
        {
            if (x > 0)
            {
                x *= -1;
            }
        }
        if (rb.velocity.z > maxSpeed)
        {
            if (z > 0)
            {
                z *= -1;
            }
        }

        rb.AddForce(x, 0f, z);
    }

    public override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
    }
}
