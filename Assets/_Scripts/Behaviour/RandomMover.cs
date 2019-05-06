using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class RandomMover : EnergyBall
{
    public ShipFloatConfig floatConfig;
    private float floatTopBound, floatBottomBound;
    private bool upperBoundReached;
    private float currentBaseHeight;

    public float maxVelocity;

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
        currentBaseHeight = floatConfig.baseHeightFromGround;
    }

    // Use this for initialization
    public override void Start()
    {
        base.Start();

        if (PhotonNetwork.IsMasterClient)
        {
            InitFloatSettings();

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

        y = ApplyFloating();

        // Add the calculated force
        rb.AddForce(x, y, z);
    }

    #region Floating
    private float ApplyFloating()
    {
        float floatSpeed = 0;

        // Possible cause of lag ...
        RaycastHit hit = new RaycastHit();
        if (Physics.Raycast(transform.position, -Vector3.up, out hit))
        {
            float distanceToGround = hit.distance;

            if (hit.transform.tag.Equals("Floor"))
            {

                if (distanceToGround < (floatConfig.baseHeightFromGround - 1))
                {
                    // Some smoothing
                    float floatFactor = 50 / distanceToGround;

                    // The speed to return to be used by .AddForce later on
                    //floatSpeed = floatConfig.floatSpeed * floatFactor;

                    Rigidbody shipRb = GetComponent<Rigidbody>();
                    shipRb.velocity = new Vector3(shipRb.velocity.x, floatFactor, shipRb.velocity.z);
                    floatSpeed = 0;


                    currentBaseHeight = hit.point.y + 5f;
                    floatBottomBound = currentBaseHeight - floatConfig.floatDiff;
                    floatTopBound = currentBaseHeight + floatConfig.floatDiff;
                }

                else if (hit.distance > (floatConfig.baseHeightFromGround + 1))
                {
                    float diff = currentBaseHeight - distanceToGround;

                    // Some smoothing
                    float floatFactor = 7 * distanceToGround;

                    // The speed to return to be used by .AddForce later on
                    floatSpeed = -floatConfig.floatSpeed * floatFactor;

                    Rigidbody shipRb = GetComponent<Rigidbody>();
                    shipRb.velocity = new Vector3(shipRb.velocity.x, -floatFactor, shipRb.velocity.z);
                    floatSpeed = 0;

                    //parentShip.transform.position = new Vector3(parentShip.transform.position.x, parentShip.transform.position.y - .8f, parentShip.transform.position.z);

                    currentBaseHeight = hit.point.y + 5f;
                    floatBottomBound = currentBaseHeight - floatConfig.floatDiff;
                    floatTopBound = currentBaseHeight + floatConfig.floatDiff;

                }

                else
                {
                    if (ShouldFloatUp())
                        floatSpeed = floatConfig.floatSpeed;
                    else if (ShouldFloatDown())
                        floatSpeed = -floatConfig.floatSpeed;
                }
            }
        }

        ApplyFloatBounds();

        return floatSpeed;
    }

    // Set ShipY velocity to 0 if y-pos exceeds absolute bound, and is trying to move past it
    private void ApplyFloatBounds()
    {
        // Difference between top and bottom bound; e.a. top (5.6) - bottom (4.4) = diff (1.2)
        float diff = Mathf.Round((floatTopBound - floatBottomBound) * 10) / 10;

        // Set the absolute bounds, where rigidbody force is halted if trying to move over
        float absoluteBottomBound = floatBottomBound - diff;
        float absoluteTopBound = floatTopBound + diff;

        if (transform.position.y < absoluteBottomBound)
        {
            Rigidbody shipRb = GetComponent<Rigidbody>();

            if (shipRb.velocity.y < 0)
                shipRb.velocity = new Vector3(shipRb.velocity.x, 0f, shipRb.velocity.z);
        }

        else if (transform.position.y > absoluteTopBound)
        {
            Rigidbody shipRb = GetComponent<Rigidbody>();

            if (shipRb.velocity.y > 0)
                shipRb.velocity = new Vector3(shipRb.velocity.x, 0f, shipRb.velocity.z);
        }

    }

    private float GetHeightMiddle()
    {
        float diff = floatTopBound - floatBottomBound;
        float middleHeight = floatTopBound - (diff / 2);
        return middleHeight;
    }

    private bool ShouldFloatUp()
    {
        if (transform.position.y < floatTopBound)
        {
            if (!upperBoundReached)
            {
                return true;
            }

        }

        upperBoundReached = true;
        return false;
    }
    private bool ShouldFloatDown()
    {
        if (transform.position.y > floatBottomBound)
        {
            if (upperBoundReached)
            {
                return true;
            }
        }

        upperBoundReached = false;
        return false;
    }
    #endregion

    #region Initialisations
    private void InitFloatSettings()
    {
        floatTopBound = transform.position.y + floatConfig.floatDiff;
        floatBottomBound = transform.position.y - floatConfig.floatDiff;
    }
    #endregion

    public override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
    }
}
