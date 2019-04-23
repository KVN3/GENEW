using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct ShipFloatConfig
{
    public float floatDiff;
    public float floatSpeed;
}

[System.Serializable]
public struct ShipMovementConfig
{
    public float movementSpeedFactor;
    public float rotationSpeedFactor;

    public float baseMaxSpeed;
    public float minDrag;
    public float maxDrag;

    public float initialSlowDownFactor;
}

public class ShipMovement : ShipComponent
{
    public ShipFloatConfig floatConfig;
    public ShipMovementConfig config;

    // Run data
    private float currentSpeed;
    private float currentMaxSpeed;
    private float floatTopBound, floatBottomBound;
    private bool upperBoundReached;

    public void Awake()
    {
        currentSpeed = 0f;

    }

    public void Start()
    {
        currentMaxSpeed = config.baseMaxSpeed;
        InitFloatSettings();
    }

    #region Movement
    public void Move(Vector3 force, float verticalInput, float horizontalInput)
    {
        Rigidbody rb = parentShip.GetComponent<Rigidbody>();
        Vector3 vel = rb.velocity;
        Vector3 localVel = parentShip.transform.InverseTransformVector(vel);

        // Get current speed
        currentSpeed = GetCurrentSpeed(vel);

        // Apply floating
        force.y = ApplyFloating();

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

        //Debug.Log("Vehicle speed (" + localVel.z + ") = " + currentSpeed + " MAX = " + currentMaxSpeed);
    }

    public float GetCurrentSpeed(Vector3 vel)
    {
        Vector3 localVel = parentShip.transform.InverseTransformVector(vel);
        float currSpeed = localVel.z;

        if (localVel.z < 0)
            currSpeed *= -1;

        return currSpeed;
    }

    public void GivingGas()
    {
        Rigidbody rb = parentShip.GetComponent<Rigidbody>();

        parentShip.components.engines.middleEngine.Activate();

        if (rb.drag > config.minDrag)
            rb.drag -= 0.3f;

        if (rb.angularDrag > 0)
            rb.angularDrag -= 0.2f;

        if (rb.drag < config.minDrag)
            rb.drag = config.minDrag;
    }

    public void NotGivingGas()
    {
        Rigidbody rb = parentShip.GetComponent<Rigidbody>();

        parentShip.components.engines.middleEngine.Deactivate();

        if (rb.drag < config.maxDrag)
        {
            float slowDownFactor = config.initialSlowDownFactor;
            if (rb.drag >= 1)
                slowDownFactor *= rb.drag;

            rb.drag += slowDownFactor;
        }
    }

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
    public void Rotate(Vector3 acceleration, float horizontalInput, MovementState sideMovementState)
    {
        Rigidbody rb = parentShip.GetComponent<Rigidbody>();

        // Worldspace Vel -> Local Vel
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

    public void ResetAngleZ(float addValue)
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

    #region Floating
    private float ApplyFloating()
    {
        float floatSpeed = 0;

        if (ShouldFloatUp())
            floatSpeed = floatConfig.floatSpeed;
        else if (ShouldFloatDown())
            floatSpeed = -floatConfig.floatSpeed;

        ApplyFloatingBounds();

        return floatSpeed;
    }

    private void ApplyFloatingBounds()
    {
        float diff = Mathf.Round((floatTopBound - floatBottomBound) * 10) / 10;

        if (parentShip.transform.position.y < (floatBottomBound - diff))
        {
            parentShip.transform.position = new Vector3(parentShip.transform.position.x, floatBottomBound, parentShip.transform.position.z);
        }
        else if (parentShip.transform.position.y > (floatTopBound + diff))
        {
            parentShip.transform.position = new Vector3(parentShip.transform.position.x, floatTopBound, parentShip.transform.position.z);
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
        if (parentShip.transform.position.y < floatTopBound)
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
        if (parentShip.transform.position.y > floatBottomBound)
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

    #region Initialisations
    private void InitFloatSettings()
    {
        floatTopBound = parentShip.transform.position.y + floatConfig.floatDiff;
        floatBottomBound = parentShip.transform.position.y - floatConfig.floatDiff;
    }
    #endregion

    #region ItemActions
    public void ActivateSpeedBoost(float maxSpeedIncrease, float boostFactor, float boostDuration)
    {
        parentShip.score += 300;
        shipSoundManager.PlaySound(SoundType.SPEEDBOOST);
        StartCoroutine(ApplySpeedBoost(maxSpeedIncrease, boostFactor, boostDuration));
    }

    private IEnumerator ApplySpeedBoost(float maxSpeedIncrease, float boostFactor, float boostDuration)
    {
        Rigidbody rb = parentShip.GetComponent<Rigidbody>();

        // Increase max speed & set boost color
        currentMaxSpeed += maxSpeedIncrease;
        parentShip.components.engines.middleEngine.SetBoostColor();
        parentShip.components.engines.leftEngine.SetBoostColor();
        parentShip.components.engines.rightEngine.SetBoostColor();
        
        Vector3 newVelocity = new Vector3(rb.velocity.x * boostFactor, rb.velocity.y, rb.velocity.z * boostFactor);
        rb.velocity = newVelocity;

        yield return new WaitForSeconds(boostDuration);

        // Restore max speed & restore color
        currentMaxSpeed -= maxSpeedIncrease;
        parentShip.components.engines.middleEngine.RestoreColor();
        parentShip.components.engines.leftEngine.RestoreColor();
        parentShip.components.engines.rightEngine.RestoreColor();
    }


    #endregion
}
