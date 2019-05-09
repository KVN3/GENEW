using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(HoveringManager))]
public class RandomMover : EnergyBall
{
    public float maxVelocity;

    [SerializeField]
    private HoveringManager hoveringManager;

    public Shooter shooterModule;

    public float xMax;
    public float zMax;
    public float xMin;
    public float zMin;
    public float maxSpeed;

    private float x;
    private float z;
    private float y;

    private float tmp;
    private float angle;
    private Rigidbody rb;


    // Awake
    public override void Awake()
    {
        base.Awake();
    }

    // Use this for initialization
    public override void Start()
    {
        base.Start();

        if (PhotonNetwork.IsMasterClient)
        {
            x = Random.Range(-maxVelocity, maxVelocity);
            z = Random.Range(-maxVelocity, maxVelocity);
            angle = Mathf.Atan2(x, z) * (180 / 3.141592f) + 90;
            transform.localRotation = Quaternion.Euler(0, angle, 0);
        }

    }
    void Update()
    {

        // Handle the movement is master client, else only read the new values and smoothly move to the right position
        if (PhotonNetwork.IsMasterClient)
            Move();
        else
            SmoothMove();
    }

    // Random movement within set bounds
    private void Move()
    {
        rb = GetComponent<Rigidbody>();
        tmp += Time.deltaTime;

        // If local [axis] is higher than the [axis] bound, force can only be applied negative [bound], and the other way around
        if (transform.localPosition.x > xMax)
        {
            x = Random.Range(-maxVelocity, 0.0f);
            tmp = 0.0f;
        }
        if (transform.localPosition.x < xMin)
        {
            x = Random.Range(0.0f, maxVelocity);
            tmp = 0.0f;
        }
        if (transform.localPosition.z > zMax)
        {
            z = Random.Range(-maxVelocity, 0.0f);
            tmp = 0.0f;
        }
        if (transform.localPosition.z < zMin)
        {
            z = Random.Range(0.0f, maxVelocity);
            tmp = 0.0f;
        }

        // If the object hasn't reached any of the bounds in a while, then they can move wherever
        if (tmp > 1.0f)
        {
            x = Random.Range(-maxVelocity, maxVelocity);
            z = Random.Range(-maxVelocity, maxVelocity);
            tmp = 0.0f;
        }

        // Apply negative force if going over maxspeed
        if (rb.velocity.x > maxSpeed)
            if (x > 0)
                x *= -1;
        if (rb.velocity.z > maxSpeed)
            if (z > 0)
                z *= -1;

        // Floating force
        y = hoveringManager.ApplyRaycastHovering();

        // Add the calculated force
        rb.AddForce(x, y, z);
    }

    public override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
    }
}
