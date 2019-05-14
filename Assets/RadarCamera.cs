using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadarCamera : MonoBehaviour
{
    public PlayerShip target;
    public float damping;

    public float offsetX, offsetY, offsetZ;


    private Vector3 offset;
    private float angleZ = 60f;

    void Start()
    {
        
    }

    void LateUpdate()
    {
        offset = new Vector3(offsetX, offsetY, offsetZ);

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

}
