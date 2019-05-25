using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public enum HoverDirection
{
    UP, DOWN
}

[System.Serializable]
public struct HoveringConfig
{
    [Tooltip("Top/bottom bound difference to base height.")]
    public float hoveringDiff;

    [Tooltip("Force to be applied.")]
    public float hoveringForce;

    [Tooltip("Standard object height (Y) from ground. Only used if useUniqueBaseHeight = true.")]
    public float uniqueBaseHeight;

    [Tooltip("Only set this to true if this object requires a unique base height.")]
    public bool useUniqueBaseHeight;

    [Tooltip("Point from which distance to the ground is to be calculated.")]
    public GameObject sensor;
}

public class HoveringManager : MonoBehaviour
{
    [SerializeField]
    private HoveringConfig config;

    #region run data
    private float topBound, bottomBound;
    private bool topBoundReached;

    // Kept at about ~baseHeightFromGround off the ground
    private float baseHeight;
    private float currentBaseHeight;
    #endregion

    private void Awake()
    {
        Assert.IsNotNull(config.sensor, "Sensor not set in Hovering Manager.");
        Assert.AreNotEqual(config.hoveringDiff, 0, "Not hovering because hoveringDiff set to 0.");
        Assert.AreNotEqual(config.hoveringForce, 0, "Not hovering because hoveringForce set to 0.");

        // Set initial base height
        if (config.useUniqueBaseHeight)
            baseHeight = config.uniqueBaseHeight;
        else
            baseHeight = GameConfiguration.PlayingHeight;


        currentBaseHeight = baseHeight;

        // Set initial bounds
        topBound = currentBaseHeight + config.hoveringDiff;
        bottomBound = currentBaseHeight - config.hoveringDiff;
    }

    #region public
    // Hover between two bounds, without raycasting for height
    public float ApplyRegularHovering()
    {
        float hoveringSpeed = 0;

        // Get the force in the correct direction
        GetHoveringForce();

        // Checks and applies bounds if object Y pos exceeds them
        ApplyBounds();

        return hoveringSpeed;
    }


    // Hover between two bounds and keep at an altitude of baseHeight
    public float ApplyRaycastHovering()
    {
        float force = 0;

        RaycastHit hit = new RaycastHit();
        if (Physics.Raycast(config.sensor.transform.position, -Vector3.up, out hit))
        {
            if (hit.transform.tag.Equals("Floor"))
            {
                // If distance to ground is too low, hover upwards
                if (hit.distance < (baseHeight - 1f))
                    Hover(hit, HoverDirection.UP);

                // If distance to ground is too high, hover downwards
                else if (hit.distance > (baseHeight + 1f))
                    Hover(hit, HoverDirection.DOWN);

                // Distance to ground within bounds, hover between two values
                else
                    force = GetHoveringForce();
            }
        }

        ApplyBounds();

        return force;
    }
    #endregion

    #region private
    // Returns the hovering force in the correct direction (up or down) based on current Y pos
    private float GetHoveringForce()
    {
        float force = 0;

        if (ShouldFloatUp())
            force = config.hoveringForce;
        else if (ShouldFloatDown())
            force = -config.hoveringForce;

        return force;
    }

    // Hover directly using velocity
    private void Hover(RaycastHit hit, HoverDirection hoverDirection)
    {
        Rigidbody rigidBody = GetComponent<Rigidbody>();
        float floatFactor;

        if (hoverDirection.Equals(HoverDirection.UP))
        {
            floatFactor = 50 / hit.distance;
            rigidBody.velocity = new Vector3(rigidBody.velocity.x, floatFactor, rigidBody.velocity.z);
        }
        else
        {
            floatFactor = 2 * hit.distance;
            rigidBody.velocity = new Vector3(rigidBody.velocity.x, -floatFactor, rigidBody.velocity.z);
        }

        currentBaseHeight = hit.point.y + baseHeight;
        bottomBound = currentBaseHeight - config.hoveringDiff;
        topBound = currentBaseHeight + config.hoveringDiff;
    }

    // Set ShipY velocity to 0 if y-pos exceeds absolute bound, and is trying to move past it
    private void ApplyBounds()
    {
        // Difference between top and bottom bound; e.a. top (5.6) - bottom (4.4) = diff (1.2)
        float diff = Mathf.Round((topBound - bottomBound) * 10) / 10;

        // Set the absolute bounds, where velocity force is halted if trying to move over
        float absoluteBottomBound = bottomBound - diff;
        float absoluteTopBound = topBound + diff;

        if (transform.position.y < absoluteBottomBound)
        {
            Rigidbody shipRb = GetComponent<Rigidbody>();

            if (shipRb.velocity.y < 0)
                shipRb.velocity = new Vector3(shipRb.velocity.x, 0f, shipRb.velocity.z);
        }

        else if (transform.position.y > absoluteTopBound)
        {
            Rigidbody shipRb = GetComponent<Rigidbody>();

            if (shipRb.velocity.y > 0)
                shipRb.velocity = new Vector3(shipRb.velocity.x, 0f, shipRb.velocity.z);
        }

    }

    // Checks if object should float up
    private bool ShouldFloatUp()
    {
        if (transform.position.y < topBound)
        {
            if (!topBoundReached)
            {
                return true;
            }

        }

        topBoundReached = true;
        return false;
    }

    // Checks if object should float down
    private bool ShouldFloatDown()
    {
        if (transform.position.y > bottomBound)
        {
            if (topBoundReached)
            {
                return true;
            }
        }

        topBoundReached = false;
        return false;
    }
    #endregion
}
