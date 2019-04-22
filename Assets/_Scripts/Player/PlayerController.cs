using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

[System.Serializable]
public struct Cameras
{
    public GameObject firstPersonCamera;
    public GameObject thirdPersonCamera;
}

public class PlayerController : MonoBehaviour
{
    public PlayerShip playerShip;
    public Accelerometer accelerometer;
    public Cameras cameras;

    public bool useAccelerometerControls = true;

    private PhotonView photonView;

    void Awake()
    {
        photonView = GetComponent<PhotonView>();
    }

    private void Start()
    {
        if (photonView.IsMine)
        {
            // Rotate
            float x = 0;
            float y = 90f;
            float z = 0;
            transform.localEulerAngles = new Vector3(x, y, z);
        }
    }

    private void Update()
    {
        // Prevent control if connected to Photon and represent the localPlayer
        if (photonView.IsMine == false && PhotonNetwork.IsConnected == true)
        {
            return;
        }

        if (photonView.IsMine)
        {
            HandleCameraControls();
            HandlePreferedControls();
            HandlePlayerActionControls();
        }
    }

    private void HandleCameraControls()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            cameras.firstPersonCamera.SetActive(!cameras.firstPersonCamera.activeSelf);
            cameras.thirdPersonCamera.SetActive(!cameras.thirdPersonCamera.activeSelf);
        }
    }

    private void HandlePlayerActionControls()
    {
        // Breaking
        if (Input.GetKey(KeyCode.Space))
            playerShip.components.movement.Break();

        // Shooting
        if (Input.GetKeyDown(KeyCode.E))
            playerShip.UseItem();

        // Forcefield
        // If forcefield item not active, key down, enough charges and no cooldown then activate.
        if (!playerShip.components.forcefield.IsItemActive())
        {
            if (Input.GetKey(KeyCode.C) && playerShip.components.forcefield.HasEnoughCharges())
            {
                playerShip.components.forcefield.Activated(true);
            }
            else
            {
                if (!playerShip.WasRecentlyHit())
                    playerShip.components.forcefield.Deactivated();
            }
        }
    }

    private void HandlePreferedControls()
    {
        // Controls
        if (Input.GetKeyDown(KeyCode.G))
            useAccelerometerControls = !useAccelerometerControls;
    }

    void FixedUpdate()
    {
        if (playerShip.GetComponent<PhotonView>().IsMine)
        {
            if (RaceManager.raceStarted)
                HandleMovement();
        }
    }

    private void HandleMovement()
    {
        float horizontalInput;
        float verticalInput;
        float forwardFactor = 1f;
        float rotationalFactor = 1f;

        // Accelerometer | Keyboard
        if (useAccelerometerControls)
        {
            Vector3 acceleration = accelerometer.GetValue();
            horizontalInput = accelerometer.ParseAccelerationToInput(acceleration.x, Direction.X);
            verticalInput = -accelerometer.ParseAccelerationToInput(acceleration.y, Direction.Y);
            forwardFactor = 2f;
            rotationalFactor = 2f;
        }
        else
        {
            horizontalInput = Input.GetAxis("Horizontal");
            verticalInput = Input.GetAxis("Vertical");
        }

        ApplyMovement(horizontalInput, verticalInput, forwardFactor, rotationalFactor);
    }

    private void ApplyMovement(float horizontalInput, float verticalInput, float forwardFactor, float rotationalFactor)
    {
        if (!playerShip.components.system.IsSystemDown())
        {
            MovementState sideMovementState = Movement.GetMovementState(horizontalInput, MovementType.SIDE);

            // Engines & Drag
            playerShip.components.engines.ManageEngines(horizontalInput);

            if (GivingGas(verticalInput) && !Input.GetKey(KeyCode.Space))
                playerShip.components.movement.GivingGas();
            else if (!Input.GetKey(KeyCode.Space))
                playerShip.components.movement.NotGivingGas();

            // Rotation
            if (Movement.IsNotIdle(sideMovementState))
            {
                float y = horizontalInput * playerShip.components.movement.config.rotationSpeedFactor * rotationalFactor;
                float z = horizontalInput * playerShip.components.movement.config.rotationSpeedFactor * rotationalFactor;
                playerShip.components.movement.Rotate(new Vector3(0f, y, z), horizontalInput, sideMovementState);
            }
            else
            {
                playerShip.components.movement.ResetAngleZ(1);
            }

            // Thrust
            Vector3 forward = -1 * verticalInput * transform.forward * Time.deltaTime * playerShip.components.movement.config.movementSpeedFactor * forwardFactor;
            playerShip.components.movement.Move(forward, verticalInput, horizontalInput);
        }
        else
        {
            playerShip.components.movement.NotGivingGas();
        }
    }

    private bool GivingGas(float verticalInput)
    {
        if (verticalInput > 0)
            return true;

        return false;
    }
}