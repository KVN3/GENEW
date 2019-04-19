using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPad : MonoBehaviour
{
    public Collectable[] itemClasses;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Ship"))
        {
            Ship ship = other.GetComponent<PlayerShip>();

            ship.GetShipSoundManager().PlaySound(SoundType.PICKUP);
            ship.SetItem(itemClasses[Random.Range(0, itemClasses.Length)], 1);
        }
    }
}
