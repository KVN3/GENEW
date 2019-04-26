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

    private void FixedUpdate()
    {
        if (playerShip.GetComponent<PhotonView>().IsMine)
        {
            if (RaceManager.raceStarted)
                HandleMovement();
        }
    }

    #region Control Handlers
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
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.UnloadSceneAsync("Wasteland");
            SceneManager.LoadScene("Main Menu");
        }

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

    #endregion


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

    #region Managers
    // Manage engines on/off and drag increase/decrease
    private void ManageEnginesAndDrag(float horizontalInput, float verticalInput)
    {
        // Handle left & right engine
        playerShip.components.engines.ManageEngines(horizontalInput);

        // Handle drag & front engine
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
            // Set the rotation
            float angleY = horizontalInput * playerShip.components.movement.config.rotationSpeedFactor * rotationalFactor;
            float angleZ = horizontalInput * playerShip.components.movement.config.rotationSpeedFactor * rotationalFactor;
            Vector3 rotation = new Vector3(0f, angleY, angleZ);

            // Rotate the ship along its two axis
            playerShip.components.movement.Rotate(rotation, horizontalInput, sideMovementState);
        }

        // Restore rotation
        else
        {
            playerShip.components.movement.ResetAngleZ(1);
        }
    }

    private void ManageThrust(float horizontalInput, float verticalInput, float forwardFactor)
    {
        Vector3 forward = -1 * verticalInput * transform.forward * Time.deltaTime * playerShip.components.movement.config.movementSpeedFactor * forwardFactor;
        playerShip.components.movement.Move(forward, verticalInput, horizontalInput);
    }
    
    private void ManageCamera(float verticalInput)
    {
        if(GivingGas(verticalInput))
            playerCamera.IncreaseDistance(0.05f, 0.1f);
        else
            playerCamera.DecreaseDistance(0.05f, 0.1f);
    }
    #endregion

    private bool GivingGas(float verticalInput)
    {
        if (verticalInput > 0)
            return true;

        return false;
    }

    public void SetPlayerCamera(PlayerCamera playerCamera)
    {
        this.playerCamera = playerCamera;
    }
}