using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

public class EnergyBall : MonoBehaviour
{
    public int shutDownDuration = 4;
    public EnemySoundManager enemySoundManagerClass;
    protected EnemySoundManager enemySoundManager;

    public virtual void Awake()
    {
        enemySoundManager = Instantiate(enemySoundManagerClass, transform.localPosition, transform.localRotation, this.transform);
    }

    public virtual void Start()
    {
        enemySoundManager.PlaySound(SoundType.RESTART);
    }

    public virtual void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Ship"))
        {
            PlayerShip playerShip = other.GetComponent<PlayerShip>();
            Rigidbody rb = other.GetComponent<Rigidbody>();

            playerShip.GetHitByEmp(shutDownDuration);
        }
    }

    public virtual void Die()
    {
        enemySoundManager.PlaySound(SoundType.SHUTDOWN);
        transform.DetachChildren();
        Destroy(gameObject);
    }
}
