﻿using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

[System.Serializable]
public struct ShipMovementConfig
{
    [Tooltip("Factor by which movement force is multiplied.")]
    public float movementSpeedFactor;

    [Tooltip("Factor by which rotation is multiplied.")]
    public float rotationSpeedFactor;

    [Tooltip("Ship max speed without boosts.")]
    public float baseMaxSpeed;

    public float minDrag;
    public float maxDrag;

    [Tooltip("Factor used in slowing down when not giving gas.")]
    public float initialSlowDownFactor;
}

[System.Serializable]
public struct WindTrailsConfig
{
    [Tooltip("Activation speed for trail PS.")]
    public float trailActivationSpeed;

    [Tooltip("PS Object.")]
    public GameObject windTrailsObject;
}

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(HoveringManager))]
[RequireComponent(typeof(Ship))]
public class ShipMovement : ShipComponent, IPunObservable
{
    [SerializeField]
    private ShipMovementConfig config;
    [SerializeField]
    private WindTrailsConfig trailsConfig;

    private PlayerCamera playerCamera;
    private HoveringManager hoveringManager;
    private PhotonView photonView;

    private float currentSpeed = 0f;
    private float currentMaxSpeed;

    public void Awake()
    {
        // Movement config
        Assert.AreNotEqual(config.baseMaxSpeed, 0, "baseMaxSpeed = 0");
        Assert.AreNotEqual(config.movementSpeedFactor, 0, "movementSpeedFactor = 0");
        Assert.AreNotEqual(config.rotationSpeedFactor, 0, "rotationSpeedFactor = 0");

        // Trails config
        Assert.AreNotEqual(trailsConfig.trailActivationSpeed, 0, "trailActivationSpeed = 0");
        Assert.IsNotNull(trailsConfig.windTrailsObject, "Trail object not set.");

        // Other
        //


        hoveringManager = GetComponent<HoveringManager>();
        photonView = GetComponent<PhotonView>();
    }

    public void Start()
    {
        Assert.IsNotNull(hoveringManager, "Hovering manager not set.");

        currentMaxSpeed = config.baseMaxSpeed;

        if (parentShip is PlayerShip)
        {
            PlayerShip playerShip = (PlayerShip)parentShip;
            playerCamera = playerShip.GetPlayerCamera();
        }
    }

    public void Update()
    {
        // Wind trails
        if (photonView.IsMine)
        {
            if (currentSpeed > trailsConfig.trailActivationSpeed)
            {
                trailsConfig.windTrailsObject.SetActive(true);
                windTrailsActive = true;
            }
            else
            {
                trailsConfig.windTrailsObject.SetActive(false);
                windTrailsActive = false;
            }
        }

    }

    #region Photon
    private Vector3 targetPos;
    private Quaternion targetRot;
    private bool windTrailsActive = false;

    // Send data if this is our ship, receive data if it is not
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(parentShip.transform.position);
            stream.SendNext(parentShip.transform.rotation);
            stream.SendNext(windTrailsActive);
        }
        else
        {
            targetPos = (Vector3)stream.ReceiveNext();
            targetRot = (Quaternion)stream.ReceiveNext();

            windTrailsActive = (bool)stream.ReceiveNext();

            if (windTrailsActive)
                trailsConfig.windTrailsObject.SetActive(true);
            else
                trailsConfig.windTrailsObject.SetActive(false);
        }
    }

    // Smooth moves other player ships to their correct position and rotation
    public void SmoothMove()
    {
        parentShip.transform.position = Vector3.Lerp(parentShip.transform.position, targetPos, 0.25f);
        parentShip.transform.rotation = Quaternion.RotateTowards(parentShip.transform.rotation, targetRot, 500 * Time.deltaTime);
    }

    #endregion

    #region Movement

    /// <summary>
    /// Handles ship movement.
    /// </summary>
    /// <param name="movementTypeFactor">Factor to multiply force with. Differs between control modes (keyboard / arduino).</param>
    /// <param name="verticalInput">Vertical input value. Ranges from -1 to 1, 0 being none.</param>
    /// <param name="horizontalInput">Horizontal input value. Ranges from -1 to 1, 0 being none.</param>
    public void Move(float movementTypeFactor, float verticalInput, float horizontalInput)
    {
        Rigidbody rb = parentShip.GetComponent<Rigidbody>();

        Vector3 force = -1 * verticalInput * transform.forward * Time.deltaTime * config.movementSpeedFactor * movementTypeFactor;
        Vector3 vel = rb.velocity;
        Vector3 localVel = parentShip.transform.InverseTransformVector(vel);

        // Get current speed
        currentSpeed = GetCurrentSpeed(vel);

        // Apply floating via Raycasts
        force.y = hoveringManager.ApplyRaycastHovering();

        // Add force if below current max speed, else only add force for floating.
        if (currentSpeed < currentMaxSpeed)
        {
            rb.AddForce(force);
        }
        else
        {
            Vector3 newLocalVel = localVel;
            newLocalVel.z = currentMaxSpeed;

            if (localVel.z < 0)
                newLocalVel.z *= -1;

            rb.velocity = parentShip.transform.TransformVector(newLocalVel);

            // Keep floating, but don't increase speed...
            rb.AddForce(0, force.y, 0);
        }
    }

    // Returns the current ship speed.
    public float GetCurrentSpeed(Vector3 vel)
    {
        Vector3 localVel = parentShip.transform.InverseTransformVector(vel);
        float currSpeed = localVel.z;

        // Flip negative velocity when moving in reverse.
        if (localVel.z < 0)
            currSpeed *= -1;

        return currSpeed;
    }

    // Called when giving gas, apply drag modifiers.
    public void GivingGas()
    {
        Rigidbody rb = parentShip.GetComponent<Rigidbody>();

        if (rb.drag > config.minDrag)
            rb.drag -= 0.3f;

        if (rb.angularDrag > 0)
            rb.angularDrag -= 0.2f;

        if (rb.drag < config.minDrag)
            rb.drag = config.minDrag;
    }

    // Called when not currently giving gas, applies drag modifiers.
    public void NotGivingGas()
    {
        Rigidbody rb = parentShip.GetComponent<Rigidbody>();

        if (rb.drag < config.maxDrag)
        {
            float slowDownFactor = config.initialSlowDownFactor;
            if (rb.drag >= 1)
                slowDownFactor *= rb.drag;

            rb.drag += slowDownFactor;
        }
    }

    // Called when breaking, apply drag modifiers.
    public void Break()
    {
        Rigidbody rb = parentShip.GetComponent<Rigidbody>();

        if (rb.drag < config.maxDrag)
            rb.drag += 0.2f;

        if (rb.angularDrag < config.maxDrag)
            rb.angularDrag += 0.2f;
    }
    #endregion

    #region Rotation

    /// <summary>
    /// Handles ship rotation.
    /// </summary>
    /// <param name="movementTypeFactor">Factor to multiply force with. Differs between control modes (keyboard / arduino).</param>
    /// <param name="horizontalInput">Horizontal input value. Ranges from -1 to 1, 0 being none.</param>
    /// <param name="sideMovementState">State of movement, in this case sideways movement being either: LEFT, RIGHT, IDLE.</param>
    public void Rotate(float movementTypeFactor, float horizontalInput, MovementState sideMovementState)
    {
        Rigidbody rb = parentShip.GetComponent<Rigidbody>();

        // Set the rotation
        float angleY = horizontalInput * config.rotationSpeedFactor * movementTypeFactor;
        float angleZ = horizontalInput * config.rotationSpeedFactor * movementTypeFactor;
        Vector3 acceleration = new Vector3(0f, angleY, angleZ);

        // Worldspace Vel to Local Vel
        Vector3 vel = rb.velocity;
        Vector3 localVel = parentShip.transform.InverseTransformVector(vel);

        // Rotate
        float x = parentShip.transform.localEulerAngles.x;
        float y = parentShip.transform.localEulerAngles.y + acceleration.y;
        float z = GetAngleZ(sideMovementState, acceleration.z);
        parentShip.transform.localEulerAngles = new Vector3(x, y, z);

        // Local Vel -> Worldspace Vel
        vel = parentShip.transform.TransformVector(localVel);
        rb.velocity = vel;
    }

    // Called when side movement idle, rotates back to the center by addValue degrees.
    public void RestoreAngleZ(float addValue)
    {
        float angleZ = parentShip.transform.localEulerAngles.z;

        if (angleZ > 3 && angleZ <= 180)
        {
            parentShip.transform.Rotate(0f, 0f, -addValue);
        }
        else if (angleZ <= 357 && angleZ > 180)
        {
            parentShip.transform.Rotate(0f, 0f, addValue);
        }
    }

    /// <summary>
    /// Get the current ship Z angle.
    /// </summary>
    /// <param name="sideMovementState">Movement state. Is the ship going LEFT, RIGHT or is the sideways movement IDLE</param>
    /// <param name="addValue">How much rotation to add in degrees.</param>
    /// <returns></returns>
    private float GetAngleZ(MovementState sideMovementState, float addValue)
    {
        float angleZ = parentShip.transform.localEulerAngles.z;

        float z = parentShip.transform.localEulerAngles.z;

        if ((angleZ < 20 || angleZ > 300) && sideMovementState.Equals(MovementState.RIGHT))
        {
            z = z + addValue;
        }
        else if ((angleZ > 340 || angleZ <= 50) && sideMovementState.Equals(MovementState.LEFT))
        {
            z = z + addValue;
        }

        return z;
    }

    #endregion

    #region GetSet
    public float GetCurrentSpeed()
    {
        return currentSpeed;
    }

    public float GetCurrentMaxSpeed()
    {
        return currentMaxSpeed;
    }

    public void SetCurrentMaxSpeed(float maxSpeed)
    {
        this.currentMaxSpeed = maxSpeed;
    }

    public bool IsBoosted()
    {
        if (currentMaxSpeed > config.baseMaxSpeed)
            return true;

        return false;
    }
    #endregion

    #region ItemActions

    // Activates speed boost based on passed along values
    public void ActivateSpeedBoost(float maxSpeedIncrease, float boostFactor, float boostDuration)
    {
        // For achievement check
        parentShip.usedBoost = true;
        parentShip.boostUses++;
        parentShip.totalBoostUses++;
        AchievementManager.UpdateAchievement(19, 1f);

        if (playerCamera != null)
        {
            playerCamera.ActivateBoostedCamera();
        }

        shipSoundManager.PlaySound(SoundType.SPEEDBOOST);
        StartCoroutine(ApplySpeedBoost(maxSpeedIncrease, boostFactor, boostDuration));
    }

    // Temporarily applies speed boost to ship
    private IEnumerator ApplySpeedBoost(float maxSpeedIncrease, float boostFactor, float boostDuration)
    {
        Rigidbody rb = parentShip.GetComponent<Rigidbody>();

        // Increase max speed & set boost color
        currentMaxSpeed += maxSpeedIncrease;
        parentShip.components.engines.SetBoosted(true);

        // Speed boost
        Vector3 newVelocity = new Vector3(rb.velocity.x * boostFactor, rb.velocity.y, rb.velocity.z * boostFactor);
        rb.velocity = newVelocity;

        // Wait boostDuration seconds
        yield return new WaitForSeconds(boostDuration);

        // Restore max speed & restore color
        currentMaxSpeed -= maxSpeedIncrease;
        parentShip.components.engines.SetBoosted(false);
    }

    //TO DO: OFF AFTER ON SETBOOSTED ENGINES
    #endregion
}
