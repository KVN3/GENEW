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

            Collectable item = itemClasses[Random.Range(0, itemClasses.Length)];
            int amount = 1;

            if (item is JammerProjectile)
            {
                amount = 3;
            }

            if (item is JammerMine)
            {
                amount = 3;
            }

            ship.GetShipSoundManager().PlaySound(SoundType.PICKUP);
            ship.SetItem(item, amount);
        }
    }
}
