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
        if(playSoundOnStart)
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
            PlayerShip playerShip = other.GetComponent<PlayerShip>();
            Rigidbody rb = other.GetComponent<Rigidbody>();

            playerShip.GetHitByEmp(shutDownDuration, "Energy ball");
            Die();
        }

        if (other.gameObject.CompareTag("SteppingStone"))
        {
            Die();
        }
    }

    public virtual void Die()
    {
        enemySoundManager.PlaySound(SoundType.SHUTDOWN);
        transform.DetachChildren();

        if (manager != null)
        {
            manager.RemoveFromAliveEnemies(this);
        }

        PhotonNetwork.Destroy(gameObject);
    }

    public void SetManager(EnemyManager manager)
    {
        this.manager = manager;
    }

    #region Photon
    private Vector3 targetPos;
    private Quaternion targetRot;

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

    public void SmoothMove()
    {
        transform.position = Vector3.Lerp(transform.position, targetPos, 0.25f);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, 500 * Time.deltaTime);
    }
    #endregion
}
