using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Chaser : EnergyBall
{
    public float maxForce;
    public float minDistance = 400000;

    private EnemyManager manager;
    public PlayerShip[] targets;

    private Vector3 moveDirection;
    private bool isCloseEnough = false;

    public void Move(float force)
    {
        if (isCloseEnough)
        {
            Rigidbody rigidbody = this.GetComponent<Rigidbody>();
            rigidbody.AddForce(moveDirection * force);
        }
    }

    public Transform GetClosestTarget()
    {
        bool isCloseEnough = false;

        Transform closestTarget = this.transform;
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = transform.position;

        foreach (PlayerShip target in targets)
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

        if (isCloseEnough)
            this.isCloseEnough = true;
        else
            this.isCloseEnough = false;

        return closestTarget;
    }

    public void SetMoveDirection(Vector3 moveDirection)
    {
        this.moveDirection = moveDirection;
    }

    public void SetManager(EnemyManager manager)
    {
        this.manager = manager;
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
            enemySoundManager.PlaySound(SoundType.SHUTDOWN);
            Destroy(gameObject);
            manager.RemoveFromAliveEnemies(this);
        }
    }
}