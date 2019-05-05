﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Chaser : EnergyBall
{
    public ShipFloatConfig floatConfig;
    private float floatTopBound, floatBottomBound;
    private bool upperBoundReached;
    private float currentBaseHeight;

    public float maxForce;
    public float minDistance = 400000;
    public PlayerShip[] targets;

    private Vector3 moveDirection;
    private bool isCloseEnough = false;

    private float closestDistanceSqr;

    public override void Awake()
    {
        base.Awake();

        currentBaseHeight = floatConfig.baseHeightFromGround;
    }

    public override void Start()
    {
        base.Start();
        InitFloatSettings();
    }

    public void Move(float force)
    {
        if(closestDistanceSqr < 40000)
        {
            force = force / 2;
        }

        Rigidbody rigidbody = this.GetComponent<Rigidbody>();
        if (isCloseEnough)
        {

            rigidbody.AddForce(moveDirection * force);
        }
        else
        {
            rigidbody.velocity = Vector3.zero;
        }
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

                if (distanceToGround < 4f)
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

                else if (hit.distance > 6f)
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

                    if (hit.distance > 5.3f)
                    {
                        currentBaseHeight = hit.point.y + 5f;
                        floatBottomBound = currentBaseHeight - floatConfig.floatDiff;
                        floatTopBound = currentBaseHeight + floatConfig.floatDiff;
                    }
                    //Debug.Log("New base height = " + currentBaseHeight);
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

    public Transform GetClosestTarget()
    {
        bool isCloseEnough = false;

        Transform closestTarget = this.transform;
        closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = transform.position;

        foreach (PlayerShip target in targets)
        {
            Transform potentialTarget = target.transform;

            Vector3 directionToTarget = potentialTarget.position - currentPosition;

            float dSqrToTarget = directionToTarget.sqrMagnitude;

            if (dSqrToTarget < closestDistanceSqr)
            {
                closestDistanceSqr = dSqrToTarget;

                if (closestDistanceSqr < minDistance)
                {
                    isCloseEnough = true;
                    closestTarget = potentialTarget;
                }
            }
        }

        if (isCloseEnough)
            this.isCloseEnough = true;
        else
            this.isCloseEnough = false;

        return closestTarget;
    }

    public void SetMoveDirection(Vector3 moveDirection)
    {
        this.moveDirection = moveDirection;
    }

    public void SetTargets(PlayerShip[] targets)
    {
        this.targets = targets;
    }

    public override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);

        if (other.gameObject.CompareTag("Ship"))
        {
            Die();
            if (manager != null)
                manager.RemoveFromAliveEnemies(this);
        }
    }
}