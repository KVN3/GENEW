using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floater : MonoBehaviour, IPunObservable
{
    [SerializeField]
    private float floatSpeed = 10f;
    public void SetFloatSpeed(float floatSpeed)
    {
        this.floatSpeed = floatSpeed;
    }

    [SerializeField]
    private Direction direction;
    public void SetDirection(Direction direction)
    {
        this.direction = direction;
    }

    // Update is called once per frame
    void Update()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Float();
        }
        else
        {
            SmoothMove();
        }
    }

    private void Float()
    {
        Rigidbody rb = this.GetComponent<Rigidbody>();

        switch (direction)
        {
            case Direction.X:
                rb.AddForce(new Vector3(floatSpeed, 0f, 0f));
                break;
            case Direction.Y:
                rb.AddForce(new Vector3(0f, floatSpeed, -(floatSpeed / 2)));
                break;
            case Direction.Z:
                rb.AddForce(new Vector3(0f, 0f, floatSpeed));
                break;
        }
    }

    #region Photon
    private Vector3 targetPos;
    private Quaternion targetRot;

    // Send data if this is our ship, receive data if it is not
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        else
        {
            targetPos = (Vector3)stream.ReceiveNext();
            targetRot = (Quaternion)stream.ReceiveNext();
        }
    }

    // Smooth moves other player ships to their correct position and rotation
    public void SmoothMove()
    {
        transform.position = Vector3.Lerp(transform.position, targetPos, 0.25f);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, 500 * Time.deltaTime);
    }
    #endregion

    
}
