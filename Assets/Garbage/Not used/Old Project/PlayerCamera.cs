using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MyMonoBehaviour
{
    public PlayerShip target;
    public float damping;

    public float minOffsetX = 3f;
    public float maxOffsetX = 14f;

    private Vector3 offset;
    private float angleZ = 60f;

    void Start()
    {
        offset = new Vector3(minOffsetX, -17f, 0f);
    }

    void LateUpdate()
    {
        // Get the angle from current and desired
        float currentAngle = transform.eulerAngles.y;
        float desiredAngle = target.transform.eulerAngles.y;
        float angle = Mathf.LerpAngle(currentAngle, desiredAngle, Time.deltaTime * damping);

        // Rotation difference
        Quaternion rotation = Quaternion.Euler(0, angle + 90f, angleZ);

        // Sets the new position based on player movement and rotation
        transform.position = target.transform.position - (rotation * offset);

        // Rotate the camera to keep looking at the player
        transform.LookAt(target.transform);
    }

    // Moves the camera further away from the player
    public void IncreaseDistance(float factorOffsetX, float factorAngleZ)
    {
        if (offset.x < 14f)
        {
            offset.x += factorOffsetX;
            angleZ -= factorAngleZ;
        }
    }

    // Brings the camera closer to the player
    public void DecreaseDistance(float factorOffsetX, float factorAngleZ)
    {
        if (offset.x > 8f)
        {
            offset.x -= factorOffsetX;
            angleZ += factorAngleZ;
        }
    }

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
}

//// if(target.components.movement.GetCurrentSpeed() > 150)
//        {
//            thisPos.z += 10;
//        }
// TO DO FINISH CAMERA