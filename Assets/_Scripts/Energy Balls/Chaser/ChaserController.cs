using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaserController : MonoBehaviour
{
    public Chaser chaser;

    void Start()
    {
        StartCoroutine(PerformMovement());
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        float moveForce = Random.Range(chaser.maxForce - 20, chaser.maxForce);
        chaser.Move(moveForce);
    }

    //--MOVEMENT--
    IEnumerator PerformMovement()
    {
        while (true)
        {
            Vector3 target = FindTarget();
            yield return MoveToTarget(target);
        }
    }

    IEnumerator MoveToTarget(Vector3 target)
    {
        Vector3 diff = target - this.transform.position;
        chaser.SetMoveDirection(diff.normalized);

        yield return new WaitForFixedUpdate();
    }

    // Methods
    private Vector3 FindTarget()
    {
        return chaser.GetClosestTarget().position;
    }
}
