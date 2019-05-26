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
            if (ScenesInformation.IsTutorialScene())
            {
                TutorialManager.Instance.RocketPickedUp = true;
            }

            Ship ship = other.GetComponent<PlayerShip>();

            Collectable item = itemClasses[Random.Range(0, itemClasses.Length)];
            int amount = GetAmount(item);

            ship.GetShipSoundManager().PlaySound(SoundType.PICKUP);
            ship.SetItem(item, amount);
        }
    }

    private int GetAmount(Collectable item)
    {
        int amount = 1;

        if (item is JammerProjectile)
            amount = 3;

        else if (item is JammerMine)
            amount = 3;
        else if (item is SmokeScreenItem)
            amount = 2;

        return amount;
    }
}
