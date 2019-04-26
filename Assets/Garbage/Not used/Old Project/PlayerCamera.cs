using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MyMonoBehaviour
{
    public PlayerShip target;
    public float damping = 1;

    public float minOffsetX = 3f;
    public float maxOffsetX = 14f;

    private Vector3 offset;
    private float angleZ = 60f;

    void Start()
    {
        //basePos = this.transform;
        offset = new Vector3(minOffsetX, -17f, 0f);
    }

    void LateUpdate()
    {
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
            offset.x += factorOffsetX;
            angleZ -= factorAngleZ;
        }
    }

    public void DecreaseDistance(float factorOffsetX, float factorAngleZ)
    {
        if (offset.x > 8f)
        {
            offset.x -= factorOffsetX;
            angleZ += factorAngleZ;
        }
    }

    public void ActivateBoostedCamera()
    {
        StartCoroutine(BoostedCamera());
    }

    private IEnumerator BoostedCamera()
    {
        int totalIterations = 20;
        float factorOffsetX = 5f / totalIterations;
        float factorAngleZ = 10f / totalIterations;

        for (int i = 0; i < totalIterations; i++)
        {
            IncreaseDistance(factorOffsetX, factorAngleZ);
            yield return new WaitForSeconds(1f / totalIterations);
        }

        for (int i = 0; i < totalIterations; i++)
        {
            DecreaseDistance(factorOffsetX, factorAngleZ);
            yield return new WaitForSeconds(1f / totalIterations);
        }
    }
}

//// if(target.components.movement.GetCurrentSpeed() > 150)
//        {
//            thisPos.z += 10;
//        }
// TO DO FINISH CAMERA