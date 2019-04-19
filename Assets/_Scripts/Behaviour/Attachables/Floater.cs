using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floater : MonoBehaviour
{
    private float floatSpeed = 10f;
    private Direction direction;

    // Update is called once per frame
    void Update()
    {
        Rigidbody rb = this.GetComponent<Rigidbody>();

        switch (direction)
        {
            case Direction.X:
                rb.AddForce(new Vector3(floatSpeed, 0f, 0f));
                break;
            case Direction.Y:
                rb.AddForce(new Vector3(0f, floatSpeed, -(floatSpeed / 2)));
                break;
            case Direction.Z:
                rb.AddForce(new Vector3(0f, 0f, floatSpeed));
                break;
        }

    }

    public void SetDirection(Direction direction)
    {
        this.direction = direction;
    }

    public void SetFloatSpeed(float floatSpeed)
    {
        this.floatSpeed = floatSpeed;
    }
}
