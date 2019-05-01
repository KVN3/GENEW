using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct ShipFloatConfig
{
    public float floatDiff;
    public float floatSpeed;
    public float baseHeightFromGround;
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

    public float trailActivationSpeed;
}

public class ShipMovement : ShipComponent, IPunObservable
{
    public ShipFloatConfig floatConfig;
    public ShipMovementConfig config;

    [SerializeField]
    private GameObject windTrailsObject;
    [SerializeField]
    private GameObject groundSensor;

    private PlayerCamera playerCamera;

    // Run data
    private float currentSpeed;
    private float currentMaxSpeed;
    private float floatTopBound, floatBottomBound;
    private bool upperBoundReached;

    private float currentBaseHeight;

    public void Awake()
    {
        currentSpeed = 0f;
        currentBaseHeight = floatConfig.baseHeightFromGround;
    }

    public void Start()
    {
        currentMaxSpeed = config.baseMaxSpeed;
        InitFloatSettings();

        if (parentShip is PlayerShip)
        {
            PlayerShip playerShip = (PlayerShip)parentShip;
            playerCamera = playerShip.GetPlayerCamera();
        }
    }

    public void Update()
    {
        //RaycastHit hit = new RaycastHit();
        //if (Physics.Raycast(transform.position, -Vector3.up, out hit))
        //{
        //    var distanceToGround = hit.distance;

        //    if (hit.transform.tag.Equals("Floor"))
        //    {
        //        if (hit.distance < (floatBottomBound - 1.5f))
        //        {
        //            float diff = currentBaseHeight - hit.distance;

        //            currentBaseHeight += .1f;
        //            floatBottomBound = currentBaseHeight - floatConfig.floatDiff;
        //            floatTopBound = currentBaseHeight + floatConfig.floatDiff;
        //            Debug.Log("New base height = " + currentBaseHeight);
        //        }
        //        //if (hit.distance < (floatBottomBound - 1.5f) || hit.distance > (floatTopBound + 1.5f))
        //        //{

        //        //}
        //    }

        //    //Debug.Log(hit.distance);
        //}

        if (GetComponent<PhotonView>().IsMine)
        {
            if (currentSpeed > config.trailActivationSpeed)
            {
                windTrailsObject.SetActive(true);
                windTrailsActive = true;
            }
            else
            {
                windTrailsObject.SetActive(false);
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
                windTrailsObject.SetActive(true);
            else
                windTrailsObject.SetActive(false);
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
    public void Move(Vector3 force, float verticalInput, float horizontalInput)
    {
        Rigidbody rb = parentShip.GetComponent<Rigidbody>();
        Vector3 vel = rb.velocity;
        Vector3 localVel = parentShip.transform.InverseTransformVector(vel);

        // Get current speed
        currentSpeed = GetCurrentSpeed(vel);

        // Apply floating
        //if(rb.velocity.y >)
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



        // Possible cause of lag ...
        RaycastHit hit = new RaycastHit();
        if (Physics.Raycast(groundSensor.transform.position, -Vector3.up, out hit))
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

                    Rigidbody shipRb = parentShip.GetComponent<Rigidbody>();
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

                    Rigidbody shipRb = parentShip.GetComponent<Rigidbody>();
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

        if (parentShip.transform.position.y < absoluteBottomBound)
        {
            Rigidbody shipRb = parentShip.GetComponent<Rigidbody>();

            if (shipRb.velocity.y < 0)
                shipRb.velocity = new Vector3(shipRb.velocity.x, 0f, shipRb.velocity.z);
        }

        else if (parentShip.transform.position.y > absoluteTopBound)
        {
            Rigidbody shipRb = parentShip.GetComponent<Rigidbody>();

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

    // Activates speed boost based on passed along values
    public void ActivateSpeedBoost(float maxSpeedIncrease, float boostFactor, float boostDuration)
    {
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
