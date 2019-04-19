using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipSystem : ShipComponent
{
    private bool systemsDown;

    public IEnumerator TemporaryShutDown(int duration)
    {
        if (!IsSystemDown())
        {
            ShutDown();
            yield return new WaitForSeconds(duration);
            parentShip.components.engines.RestoreSystem();
        }
    }

    public void ShutDown()
    {
        shipSoundManager.PlaySound(SoundType.SHUTDOWN);
        systemsDown = true;

        Rigidbody rb = parentShip.GetComponent<Rigidbody>();
        rb.useGravity = true;

        parentShip.components.engines.TurnOffAllEngines();
    }

    public void StartUp()
    {
        shipSoundManager.PlaySound(SoundType.RESTART);
        systemsDown = false;

        Rigidbody rb = parentShip.GetComponent<Rigidbody>();
        rb.useGravity = false;
    }

    #region GetSet
    public bool IsSystemDown()
    {
        return systemsDown;
    }
    #endregion
}
