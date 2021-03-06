﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Experimental.UIElements;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class EnergyBall : MonoBehaviour, IPunObservable
{
    #region variables
    public EnemySoundManager enemySoundManagerClass;

    // Config settings
    public int shutDownDuration = 4;
    public bool playSoundOnStart = true;

    // Instances
    protected EnemySoundManager enemySoundManager;
    protected EnemyManager manager;
    protected PhotonView photonView;

    public void SetManager(EnemyManager manager)
    {
        this.manager = manager;
    }
    public EnemyManager GetManager()
    {
        return manager;
    }
    #endregion

    public virtual void Awake()
    {
        enemySoundManager = Instantiate(enemySoundManagerClass, transform.position, transform.rotation, this.transform);
        photonView = GetComponent<PhotonView>();
    }

    public virtual void Start()
    {
        if (playSoundOnStart)
            enemySoundManager.PlaySound(SoundType.RESTART);
    }

    #region collision
    public virtual void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Ship"))
        {
            OnTriggerEnter(other.collider);
        }
    }

    public virtual void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Ship"))
        {
            // Tell the ship it got hit and stunned
            PlayerShip collidingShip = other.GetComponent<PlayerShip>();
            collidingShip.components.system.TryToStun(shutDownDuration, "Energy ball");

            // Perish
            Die();
        }

        // If the floating stones float into the energy spheres, make them perish.
        else if (other.gameObject.CompareTag("SteppingStone"))
            Die();
    }

    // Make this energy sphere perish
    public virtual void Die()
    {
        if (ScenesInformation.IsTutorialScene())
        {
            if (this is RandomMover)
                TutorialManager.Instance.EnemyKilled = true;
        }

        // Detaching children (sound managers), so that they can still play the dead sound when the object is destroyed
        try
        {
            enemySoundManager.PlaySound(SoundType.SHUTDOWN);
            transform.DetachChildren();
        }
        catch (System.Exception ex)
        {
            print("detaching error in energyball + " + ex);
        }


        // If this object was spawned by a manager, remove this from the list of alive oppositions
        if (manager != null)
            manager.RemoveFromAliveEnemies(this);

        // Destroy the object if master, for all to witness...
        try
        {
            //if (photonView.ViewID == 0)
            //{
            //    Destroy(gameObject);
            //}
            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.Destroy(this.gameObject);
            }
        }
        catch
        {

        }
        //Destroy(gameObject);

        //photonView.RPC("RPC_Die", RpcTarget.AllBufferedViaServer, this);

    }

    //[PunRPC]
    //private void RPC_Die(Gect)
    //{
    //    PhotonNetwork.Destroy(this);
    //}
    #endregion

    #region Photon
    private Vector3 targetPos;
    private Quaternion targetRot;

    // Write data if master, else read data
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

    // Read data and lerp to it, smoothly (CLIENTS)
    public void SmoothMove()
    {
        transform.position = Vector3.Lerp(transform.position, targetPos, 0.25f);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, 500 * Time.deltaTime);
    }
    #endregion
}
