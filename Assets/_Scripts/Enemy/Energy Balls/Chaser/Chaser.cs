using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Chaser : EnergyBall
{
    public float maxForce;
    public float minDistance = 400000;
    public PlayerShip[] targets;

    private Vector3 moveDirection;
    private bool isCloseEnough = false;

    private float closestDistanceSqr;

    public override void Awake()
    {
        base.Awake();
    }

    public override void Start()
    {
        base.Start();
    }

    public void Move(float force)
    {
        if (closestDistanceSqr < 40000)
        {
            force = force / 2;
        }

        Rigidbody rigidbody = this.GetComponent<Rigidbody>();
        if (isCloseEnough)
        {

            rigidbody.AddForce(moveDirection * force);
        }
        else
        {
            rigidbody.velocity = Vector3.zero;
        }
    }

    public Transform GetClosestTarget()
    {
        //List<int> removedShipIndexes = new List<int>();
        //int i = 0;

        bool isCloseEnough = false;

        Transform closestTarget = this.transform;
        closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = transform.position;

        foreach (PlayerShip target in targets)
        {
            if (target == null)
            {
                //removedShipIndexes.Add(i);
            }
            else
            {
                Transform potentialTarget = target.transform;

                Vector3 directionToTarget = potentialTarget.position - currentPosition;

                float dSqrToTarget = directionToTarget.sqrMagnitude;

                if (dSqrToTarget < closestDistanceSqr)
                {
                    closestDistanceSqr = dSqrToTarget;

                    if (closestDistanceSqr < minDistance)
                    {
                        isCloseEnough = true;
                        closestTarget = potentialTarget;
                    }
                }
            }

            //i++;
        }

        if (isCloseEnough)
            this.isCloseEnough = true;
        else
            this.isCloseEnough = false;

        // Refresh local and spawn manager ship targets if one is found to have left the game or somehow became null
        //if (removedShipIndexes.Count > 0)
        //{
        //    RefreshTargets(removedShipIndexes);
        //    manager.RefreshTargets(removedShipIndexes);
        //}
            

        return closestTarget;
    }

    //private void RefreshTargets(List<int> removedShipIndexes)
    //{
    //    foreach (int index in removedShipIndexes)
    //    {
    //        targets.RemoveAt(index);
    //    }
    //}

    public void SetMoveDirection(Vector3 moveDirection)
    {
        this.moveDirection = moveDirection;
    }

    public void SetTargets(PlayerShip[] targets)
    {
        this.targets = targets;
    }

    public override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);

        if (other.gameObject.CompareTag("Ship"))
        {
            //Die();
            //if (manager != null)
            //    manager.RemoveFromAliveEnemies(this);
        }
    }
}