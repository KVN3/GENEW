using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateAroundObject : MonoBehaviour
{
    public Transform target;
    public float orbitDistance = 10.0f;
    public float orbitDegreesPerSec = 180.0f;

    // Use this for initialization
    void Start()
    {
        transform.position = new Vector3(transform.position.x, 5, transform.position.z);
    }

    void Orbit()
    {
        if (target != null)
        {
            // Keep us at orbitDistance from target
            transform.position = target.position + (transform.position - target.position).normalized * orbitDistance;
            transform.RotateAround(target.position, Vector3.up, orbitDegreesPerSec * Time.deltaTime);

            transform.position = new Vector3(transform.position.x, 5, transform.position.z);
        }
    }

    // Call from LateUpdate if you want to be sure your
    // target is done with it's move.
    void LateUpdate()
    {
        Orbit();
    }
}