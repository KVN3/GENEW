using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Experimental.UIElements;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class EnergyBall : MonoBehaviour, IPunObservable
{
    public int shutDownDuration = 4;
    public EnemySoundManager enemySoundManagerClass;
    public bool playSoundOnStart = true;

    protected EnemySoundManager enemySoundManager;
    protected EnemyManager manager;

    public virtual void Awake()
    {
        enemySoundManager = Instantiate(enemySoundManagerClass, transform.localPosition, transform.localRotation, this.transform);
    }

    public virtual void Start()
    {
        if (playSoundOnStart)
            enemySoundManager.PlaySound(SoundType.RESTART);
    }

    public void Update()
    {
        // TO DO REMOVE DEBUGGING SOUND RANGE
        if (Input.GetKey(KeyCode.Z))
        {
            enemySoundManager.PlaySound(SoundType.RESTART);
        }
    }

    public virtual void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Ship"))
        {
            // Tell the ship it got hit and stunned
            PlayerShip collidingShip = other.GetComponent<PlayerShip>();
            collidingShip.GetStunned(shutDownDuration, "Energy ball");

            // Perish
            Die();
        }

        // If the floating stones float into the energy spheres, make them perish.
        if (other.gameObject.CompareTag("SteppingStone"))
            Die();
    }

    // Make this energy sphere perish
    public virtual void Die()
    {
        // Detaching children (sound managers), so that they can still play the dead sound when the object is destroyed
        enemySoundManager.PlaySound(SoundType.SHUTDOWN);
        transform.DetachChildren();

        // If this object was spawned by a manager, remove this from the list of alive oppositions
        if (manager != null)
            manager.RemoveFromAliveEnemies(this);

        // Destroy the object if master, for all to witness...
        if (PhotonNetwork.IsMasterClient)
            PhotonNetwork.Destroy(gameObject);
    }

    public void SetManager(EnemyManager manager)
    {
        this.manager = manager;
    }

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
