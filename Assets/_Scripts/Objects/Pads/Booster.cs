using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class Booster : MonoBehaviour
{
    public float boostFactor = 2;
    public float maxSpeedIncrease = 10;
    public float boostDuration = 3;

    // Start is called before the first frame update
    void Start()
    {
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Ship"))
        {
            PlayerShip playerShip = other.GetComponent<PlayerShip>();
            playerShip.movement.ActivateSpeedBoost(maxSpeedIncrease, boostFactor, boostDuration);
        }
    }
}

// TO DO: increase moveSpeedFactor in ship instead of using rb in booster