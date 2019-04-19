using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Rotator : MonoBehaviour
{
    public float rotationSpeed = 0.5f;

    private float x, y, z = 0f;

    void Start()
    {
        int angle = Random.Range(0, 2);

        switch (angle)
        {
            case 0:
                x = Random.Range(1, 2);
                break;
            case 1:
                y = Random.Range(1, 2);
                break;
            case 2:
                z = Random.Range(1, 2);
                break;
        }
    }

    void FixedUpdate()
    {

        transform.Rotate(x * rotationSpeed, y * rotationSpeed, z * rotationSpeed);
        //Float(new Vector3(0f, 0f, 2f));

    }

    //public void Float(Vector3 force)
    //{
    //    Rigidbody rb = this.GetComponent<Rigidbody>();
    //    rb.AddForce(force);
    //}
}
