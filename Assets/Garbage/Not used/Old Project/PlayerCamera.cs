using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerCamera : MyMonoBehaviour
{
    //public PlayerShip player;
    //private Vector3 offset;

    //float distance;
    //Vector3 playerPrevPos, playerMoveDir;

    //// Use this for initialization
    //void Start()
    //{
    //    offset = transform.position - new Vector3(player.transform.position.x, 10f, player.transform.position.z);

    //    distance = offset.magnitude;
    //    playerPrevPos = new Vector3(player.transform.position.x, 5f, player.transform.position.z);
    //}

    //void LateUpdate()
    //{
    //    Vector3 playerPos = new Vector3(player.transform.position.x, 5f, player.transform.position.z);

    //    playerMoveDir = playerPos - playerPrevPos;

    //    if (playerMoveDir != Vector3.zero)
    //    {
    //        playerMoveDir.Normalize();
    //        transform.position = playerPos - playerMoveDir * distance;

    //        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);

    //        transform.LookAt(playerPos);

    //        playerPrevPos = playerPos;
    //    }
    //}



    public PlayerShip target;
    public float damping = 1;

    public float minOffsetX = 8f;
    public float maxOffsetX = 14f;

    private Vector3 offset;
    private float angleZ;

    void Start()
    {
        //basePos = this.transform;
        offset = new Vector3(8f, -17f, 0f);
        angleZ = 45f;
    }

    void LateUpdate()
    {
        //float currentSpeed = target.components.movement.GetCurrentSpeed();

        //if (currentSpeed > 10)
        //{
        //    if (offset.x < 14f)
        //    {
        //        offset.x += .05f;
        //        angleZ -= 0.1f;
        //    }

        //}
        //else
        //{
        //    if (offset.x > 8f)
        //    {
        //        offset.x -= .05f;
        //        angleZ += 0.1f;
        //    }

        //}

        float currentAngle = transform.eulerAngles.y;
        float desiredAngle = target.transform.eulerAngles.y;

        float angle = Mathf.LerpAngle(currentAngle, desiredAngle, Time.deltaTime * damping);

        Quaternion rotation = Quaternion.Euler(0, angle + 90f, angleZ);
        Debug.Log(angleZ);


        Vector3 thisPos = target.transform.position - (rotation * offset);

        transform.position = thisPos;

        transform.LookAt(target.transform);
    }

    public void IncreaseDistance(float factorOffsetX, float factorAngleZ)
    {
        if (offset.x < 14f)
        {
            offset.x += .05f;
            angleZ -= 0.1f;
        }
    }

    public void DecreaseDistance(float factorOffsetX, float factorAngleZ)
    {
        if (offset.x > 8f)
        {
            offset.x -= .05f;
            angleZ += 0.1f;
        }
    }
}

//// if(target.components.movement.GetCurrentSpeed() > 150)
//        {
//            thisPos.z += 10;
//        }
// TO DO FINISH CAMERA