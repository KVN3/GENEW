using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class FloatingObject : MonoBehaviour
{
    public ShipFloatConfig floatConfig;

    private float floatTopBound, floatBottomBound;
    private bool upperBoundReached;

    public void Start()
    {
        floatTopBound = transform.position.y + floatConfig.floatDiff;
        floatBottomBound = transform.position.y - floatConfig.floatDiff;
    }

    public void Update()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        Vector3 force = new Vector3();
        force.y = ApplyFloating();

        rb.AddForce(force);
    }

    private float ApplyFloating()
    {
        float floatSpeed = 0;

        if (ShouldFloatUp())
            floatSpeed = floatConfig.floatSpeed;
        else if (ShouldFloatDown())
            floatSpeed = -floatConfig.floatSpeed;

        ApplyFloatingBounds();

        return floatSpeed;
    }

    private void ApplyFloatingBounds()
    {
        float diff = Mathf.Round((floatTopBound - floatBottomBound) * 10) / 10;

        if (transform.position.y < (floatBottomBound - diff))
        {
            transform.position = new Vector3(transform.position.x, floatBottomBound, transform.position.z);
        }
        else if (transform.position.y > (floatTopBound + diff))
        {
            transform.position = new Vector3(transform.position.x, floatTopBound, transform.position.z);
        }
    }

    private float GetHeightMiddle()
    {
        float diff = floatTopBound - floatBottomBound;
        float middleHeight = floatTopBound - (diff / 2);
        return middleHeight;
    }

    private bool ShouldFloatUp()
    {
        if (transform.position.y < floatTopBound)
        {
            if (!upperBoundReached)
            {
                return true;
            }

        }

        upperBoundReached = true;
        return false;
    }
    private bool ShouldFloatDown()
    {
        if (transform.position.y > floatBottomBound)
        {
            if (upperBoundReached)
            {
                return true;
            }
        }

        upperBoundReached = false;
        return false;
    }
}
