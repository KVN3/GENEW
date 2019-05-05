using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyBallProjectile : MonoBehaviour
{
    public float projSpeed;
    public float inaccuracyX;
    public float inaccuracyZ;

    public Vector3 target;
    private Vector3 moveDirection;

    private EnergyBall energyBall;

    // Start is called before the first frame update
    void Start()
    {
        energyBall = GetComponent<EnergyBall>();

        if (PhotonNetwork.IsMasterClient)
            StartCoroutine(WaitAndDestroy(10));

        Vector3 targ = new Vector3(target.x + +Random.Range(-inaccuracyX, inaccuracyX), target.y, target.z + Random.Range(-inaccuracyZ, inaccuracyZ));

        Vector3 diff = targ - this.transform.position;
        moveDirection = diff.normalized;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.LookAt(target, Vector3.down);
        transform.Rotate(90, 0, 0);

        Rigidbody rigidbody = this.GetComponent<Rigidbody>();
        rigidbody.AddForce(moveDirection * projSpeed);
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Ship"))
        {
            // Tell the ship it got hit and stunned
            PlayerShip collidingShip = other.gameObject.GetComponent<PlayerShip>();
            collidingShip.GetStunned(energyBall.shutDownDuration, "Energy projectile");
        }


        if (PhotonNetwork.IsMasterClient)
            PhotonNetwork.Destroy(gameObject);
    }

    IEnumerator WaitAndDestroy(int seconds)
    {
        yield return new WaitForSeconds(seconds);
        Destroy(gameObject);
    }
}
