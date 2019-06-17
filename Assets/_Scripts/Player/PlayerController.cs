using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

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
    private PlayerCamera playerCamera;

    private void Awake()
    {


        photonView = GetComponent<PhotonView>();
    }

    private void Start()
    {
        // Rotate
        float x = 0;
        float y = 90f;
        float z = 0;
        transform.localEulerAngles = new Vector3(x, y, z);
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

    #region ACTIONS
    private void HandleCameraControls()
    {
        // Camera type controls
        if (Input.GetKeyDown(KeyCode.V))
        {
            cameras.firstPersonCamera.SetActive(!cameras.firstPersonCamera.activeSelf);
            cameras.thirdPersonCamera.SetActive(!cameras.thirdPersonCamera.activeSelf);
        }

        // Rear / front view controls
        if (Input.GetKey(KeyCode.F) || Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
            playerCamera.ViewRear();
        else
            playerCamera.ViewFront();
    }

    private void HandlePlayerActionControls()
    {
        // Breaking
        if (Input.GetKey(KeyCode.C))
            playerShip.components.movement.Break();

        // UseItem
        if (Input.GetKeyDown(KeyCode.E))
            playerShip.UseItem();

        // Debug, get stunned
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.K))
            playerShip.components.system.TryToStun(2, "Debug activated");

        // Forcefield
        // If forcefield item not active, key down, enough charges and no cooldown then activate.
        if (!playerShip.components.forcefield.IsItemActive())
        {
            if (Input.GetKey(KeyCode.Space) && playerShip.components.forcefield.HasEnoughCharges())
            {
                playerShip.components.forcefield.Activated(true);
            }
            else
            {
                if (!playerShip.components.system.WasRecentlyHit())
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

        // If system is not down
        if (!playerShip.components.system.IsSystemDown())
        {
            // Engines & Drag
            ManageEnginesAndDrag(horizontalInput, verticalInput);

            // Rotation
            ManageRotation(horizontalInput, rotationalFactor);

            // Thrust
            ManageThrust(horizontalInput, verticalInput, forwardFactor);

            // Camera distance
            //ManageCamera(verticalInput);
        }
        else
        {
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

        // Apply rotation
        if (Movement.IsNotIdle(sideMovementState))
        {


            // Rotate the ship along its two axis
            playerShip.components.movement.Rotate(rotationalFactor, horizontalInput, sideMovementState);
        }

        // Restore rotation
        else
        {
            playerShip.components.movement.RestoreAngleZ(1);
        }
    }

    private void ManageThrust(float horizontalInput, float verticalInput, float forwardFactor)
    {
        playerShip.components.movement.Move(forwardFactor, verticalInput, horizontalInput);
    }

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