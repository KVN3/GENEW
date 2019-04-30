using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Experimental.UIElements;
using UnityEngine.SceneManagement;

public class EnergyBall : MonoBehaviour
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

        Destroy(gameObject);
    }

    public void SetManager(EnemyManager manager)
    {
        this.manager = manager;
    }
}
