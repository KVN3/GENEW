using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerCamera : MyMonoBehaviour
{
    [Serializable]
    public struct CameraTargetData
    {
        public Vector3 MovementOffset;
        public Vector3 LookAtOffset;
    }

    [Serializable]
    public struct CameraSpringData
    {
        public float Force;
        public float Damper;
    }

    public PlayerShip Target { get; set; }

    public CameraTargetData TargetData;
    public CameraSpringData SpringData;

    void FixedUpdate()
    {
        Rigidbody Body = this.GetComponent<Rigidbody>();

        Vector3 Diff = transform.position - (Target.transform.position + TargetData.MovementOffset);
        Vector3 Vel = Body.velocity;

        Vector3 force = (Diff * -SpringData.Force) - (Vel * SpringData.Damper);

        Body.AddForce(force);

        transform.LookAt(Target.transform.position + TargetData.LookAtOffset);
    }
}

