using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyBallProjectile : EnergyBall
{
    // Config
    public float projSpeed;
    public float inaccuracyX;
    public float inaccuracyZ;

    // Instances
    public Vector3 target;
    private Vector3 moveDirection;

    // Start is called before the first frame update
    public override void Awake()
    {
        base.Awake();

        // Destroy after 10 seconds
        Destroy(gameObject, 10);
    }

    private void Start()
    {

        // Set the target, including inaccuracy
        Vector3 targ = new Vector3(target.x + Random.Range(-inaccuracyX, inaccuracyX), target.y, target.z + Random.Range(-inaccuracyZ, inaccuracyZ));

        // Difference between target and the spawn location of this energy sphere
        Vector3 diff = targ - this.transform.position;

        // The move direction to be translated across the fixedupdates
        moveDirection = diff.normalized;

    }

    private void FixedUpdate()
    {
        // If master, handle movement. Else, smoothmove with received data from master object.

        // Look at, and rotate to point at the target
        transform.LookAt(target, Vector3.down);
        transform.Rotate(90, 0, 0);

        // Apply the force
        GetComponent<Rigidbody>().AddForce(moveDirection * projSpeed);

    }


}
