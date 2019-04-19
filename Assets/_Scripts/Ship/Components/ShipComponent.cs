using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipComponent : MonoBehaviour
{
    protected Ship parentShip;
    protected ShipSoundManager shipSoundManager;

    public void SetParentShip(Ship ship)
    {
        this.parentShip = ship;
    }

    public void SetShipSoundManager(ShipSoundManager sm)
    {
        this.shipSoundManager = sm;
    }
}
