using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct CameraConfiguration
{
    [Tooltip("Camera offset to target.")]
    public Vector3 offset;

    [Tooltip("Camera damping to diminish stuttering.")]
    public float damping;

    [Tooltip("Base angle at which the ship is viewed. Default 90.")]
    public float baseAngle;

    [Tooltip("Camera angle Z that will be repeated each iteration. Default 60.")]
    public float angleZ;
}

public class PlayerCamera : MyMonoBehaviour
{
    [SerializeField]
    private CameraConfiguration config;

    [Tooltip("PlayerShip target this camera will track. Leave empty for assignment.")]
    public PlayerShip target;

    // All other ships that haven't finished are spectatable.
    private List<PlayerShip> spectatableShips = new List<PlayerShip>();

    private bool isSpectating = false;

    public void Start()
    {

    }

    void Update()
    {
        if (isSpectating)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.E))
            {
                NextSpectatable();
            }
        }

    }

    void LateUpdate()
    {
        if (isSpectating && spectatableShips.Count == 0)
            return;

        // Get the angle from current and desired
        float currentAngle = transform.eulerAngles.y;
        float desiredAngle = target.transform.eulerAngles.y;
        float newAngle = Mathf.LerpAngle(currentAngle, desiredAngle, Time.deltaTime * config.damping);

        // Rotation difference
        Quaternion rotation = Quaternion.Euler(0, newAngle + config.baseAngle, config.angleZ);

        // Sets the new position based on player movement and rotation
        transform.position = target.transform.position - (rotation * config.offset);

        // Rotate the camera to keep looking at the player
        transform.LookAt(target.transform);
    }

    #region SPECTATOR CAMERA
    // Attempts to activate spectator mode, returns true if there's ships to spectate
    public bool ActivateSpectatorMode()
    {
        isSpectating = true;
        spectatableShips = GetSpectatables();

        if (spectatableShips.Count > 0)
        {
            target = spectatableShips[Random.Range(0, spectatableShips.Count)];

            // Camera settings
            config.offset = new Vector3(4, -70, -40);
            config.baseAngle = 60;
            config.angleZ = 55;

            return true;
        }
        else
        {
            return false;
        }
    }

    // Finds all alive ships and returns them
    private List<PlayerShip> GetSpectatables()
    {
        List<PlayerShip> spectatables = new List<PlayerShip>();

        // Find all alive ship gameObjects first
        GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("Ship");

        // Get the playership scripts from the objects and add them to the list
        for (int i = 0; i < gameObjects.Length; i++)
        {
            PlayerShip playerShip = gameObjects[i].GetComponent<PlayerShip>();

            if (playerShip.isMine)
                continue;

            spectatables.Add(playerShip);
        }

        return spectatables;
    }

    // Remove a ship from the list of spectatable ships
    public void RemoveSpectatable(PlayerShip ship)
    {
        if (isSpectating)
        {
            spectatableShips.Remove(ship);
        }
    }

    public bool SpectatablesLeft()
    {
        if (spectatableShips.Count < 1)
            return false;
        else
            return true;
    }

    // Spectate the next ship in the list
    public void NextSpectatable()
    {
        if (spectatableShips.Count <= 1)
            return;

        print("Next spectatable...");

        int currentTargetIndex = spectatableShips.IndexOf(target);
        int nextIndex = currentTargetIndex + 1;

        // Last reached, back to first
        if (nextIndex == spectatableShips.Count)
            nextIndex = 0;

        target = spectatableShips[nextIndex];
    }
    #endregion

    #region REAR/FRONT CAMERA
    // Rotate the camera to look behind
    public void ViewRear()
    {
        config.baseAngle = 270f;
    }

    public void ViewFront()
    {
        config.baseAngle = 90f;
    }
    #endregion

    #region BOOSTED CAMERA
    // Starts the boost camera effect coroutine
    public void ActivateBoostedCamera()
    {
        StartCoroutine(BoostedCamera());
    }

    // Gradually moves the camera away from the player, before returning to its average original position
    private IEnumerator BoostedCamera()
    {
        int totalIterations = 20;
        float factorOffsetX = 5f / totalIterations;
        float factorAngleZ = 10f / totalIterations;

        for (int i = 0; i < totalIterations; i++)
        {
            IncreaseDistance(factorOffsetX, factorAngleZ);
            yield return new WaitForSeconds(1f / totalIterations);
        }

        for (int i = 0; i < totalIterations; i++)
        {
            DecreaseDistance(factorOffsetX, factorAngleZ);
            yield return new WaitForSeconds(1f / totalIterations);
        }
    }

    // Moves the camera further away from the player
    private void IncreaseDistance(float factorOffsetX, float factorAngleZ)
    {
        if (config.offset.x < 14f)
        {
            config.offset.x += factorOffsetX;
            config.angleZ -= factorAngleZ;
        }
    }

    // Brings the camera closer to the player
    private void DecreaseDistance(float factorOffsetX, float factorAngleZ)
    {
        if (config.offset.x > 8f)
        {
            config.offset.x -= factorOffsetX;
            config.angleZ += factorAngleZ;
        }
    }
    #endregion
}
