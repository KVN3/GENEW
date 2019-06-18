using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;


[RequireComponent(typeof(PlayerShip))]
[RequireComponent(typeof(Accelerometer))]
public class PlayerController : MonoBehaviour
{
    private PlayerShip playerShip;
    private Accelerometer accelerometer;
    private PhotonView photonView;
    private PlayerCamera playerCamera;

    private bool useAccelerometerControls = true;

    private void Awake()
    {
        playerShip = GetComponent<PlayerShip>();
        accelerometer = GetComponent<Accelerometer>();
        photonView = GetComponent<PhotonView>();
    }

    private void Start()
    {
        // Rotate
        transform.localEulerAngles = new Vector3(0, 90f, 0);
    }

    private void Update()
    {
        if (photonView.IsMine)
        {
            HandleCameraControls();
            HandlePreferedControls();
            HandlePlayerActionControls();

            // Debug
            //if (Input.GetKeyDown(KeyCode.H))
            //{
            //    GameState.spawnEnemies = !GameState.spawnEnemies;
            //}
        }
    }

    private void FixedUpdate()
    {
        // Handle control based movement if my photonview, else just read movement data and apply
        if (photonView.IsMine)
        {
            if (RaceManager.raceStarted)
                HandleMovement();
        }
        else
        {
            playerShip.components.movement.SmoothMove();
        }
    }

    #region CONTROLS
    private void HandleCameraControls()
    {
        // Rear / front view controls
        if (Input.GetKey(KeyCode.F) || Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
            playerCamera.ViewRear();
        else
            playerCamera.ViewFront();
    }

    // Player game actions, such as item usage, breaking and forcefield
    private void HandlePlayerActionControls()
    {
        // Breaking
        if (Input.GetKey(KeyCode.C))
            playerShip.components.movement.Break();

        // UseItem
        if (Input.GetKeyDown(KeyCode.E))
            playerShip.UseItem();

        // Debug, get stunned
        //if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.K))
        //    playerShip.components.system.TryToStun(2, "Debug activated");

        // Forcefield
        // If forcefield item not active
        if (!playerShip.components.forcefield.IsItemActive())
        {
            // If key down, enough charges and no cooldown then activate.
            if (Input.GetKey(KeyCode.Space) && playerShip.components.forcefield.HasEnoughCharges())
                playerShip.components.forcefield.Activated(true);
            else
            {
                // If ship wasn't recently hit, then deactivate the forcefield
                if (!playerShip.components.system.WasRecentlyHit())
                    playerShip.components.forcefield.Deactivated();
            }
        }
    }

    // Player control preferences
    private void HandlePreferedControls()
    {
        // Toggle control mode
        if (Input.GetKeyDown(KeyCode.G))
            useAccelerometerControls = !useAccelerometerControls;
    }
    #endregion

    #region MOVEMENT
    // Handles movement input
    private void HandleMovement()
    {
        float horizontalInput;
        float verticalInput;
        float forwardFactor = 1f;
        float rotationalFactor = 1f;

        // Accelerometer | Keyboard
        if (useAccelerometerControls)
        {
            Vector3 acceleration = accelerometer.GetAcceleration();

            // Parse acceleration to horizontal and vertical input used in the scripts
            horizontalInput = accelerometer.ParseAccelerationToInput(acceleration.x, Direction.X);
            verticalInput = -accelerometer.ParseAccelerationToInput(acceleration.y, Direction.Y);

            // Compensation factor for heavier control. Likely to be tweaked in final implementation.
            forwardFactor = 2f;
            rotationalFactor = 2f;
        }
        else
        {
            horizontalInput = Input.GetAxis("Horizontal");
            verticalInput = Input.GetAxis("Vertical");
        }

        // If system is not down
        if (!playerShip.components.system.IsSystemDown())
        {
            // Engines & Drag (Which engines should be on/off and air resistance)
            ManageEnginesAndDrag(horizontalInput, verticalInput);

            // Rotation (turning ship)
            ManageRotation(horizontalInput, rotationalFactor);

            // Thrust (forward/backward movement)
            ManageThrust(horizontalInput, verticalInput, forwardFactor);
        }
        else
        {
            // Tell the ship the player's not moving
            playerShip.components.movement.NotGivingGas();
        }
    }
    #endregion

    #region MANAGERS
    // Manage engines on/off and drag increase/decrease
    private void ManageEnginesAndDrag(float horizontalInput, float verticalInput)
    {
        // Handle engines
        playerShip.components.engines.ManageEngines(horizontalInput, verticalInput);

        // Handle drag
        if (GivingGas(verticalInput) && !Input.GetKey(KeyCode.Space))
            playerShip.components.movement.GivingGas();

        else if (!Input.GetKey(KeyCode.Space))
            playerShip.components.movement.NotGivingGas();
    }

    // Rotate if needed, else restore rotation
    private void ManageRotation(float horizontalInput, float rotationalFactor)
    {
        MovementState sideMovementState = Movement.GetMovementState(horizontalInput, MovementType.SIDE);

        // Apply rotation along two axis (y for change in direction, z for turning the ship to the side)
        if (Movement.IsNotIdle(sideMovementState))
            playerShip.components.movement.Rotate(rotationalFactor, horizontalInput, sideMovementState);

        // Restore rotation to base
        else
            playerShip.components.movement.RestoreAngleZ(1);

    }

    private void ManageThrust(float horizontalInput, float verticalInput, float forwardFactor)
    {
        playerShip.components.movement.Move(forwardFactor, verticalInput, horizontalInput);
    }

    // Is the player is ordering the ship to give gas / thrust or not
    private bool GivingGas(float verticalInput)
    {
        if (verticalInput > 0)
            return true;

        return false;
    }
    #endregion

    public void SetPlayerCamera(PlayerCamera playerCamera)
    {
        this.playerCamera = playerCamera;
    }
}