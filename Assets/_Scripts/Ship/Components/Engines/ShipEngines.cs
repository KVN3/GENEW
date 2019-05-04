using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class ShipEngines : ShipComponent, IPunObservable
{
    public Engine middleEngine;
    public Engine leftEngine;
    public Engine rightEngine;

    public void Start()
    {
        Assert.IsNotNull(middleEngine);
        Assert.IsNotNull(leftEngine);
        Assert.IsNotNull(rightEngine);
        Assert.IsNotNull(parentShip);
    }

    public void FixedUpdate()
    {
        // Set boosted (green) color, if boosted. Else, restore.
        if (parentShip.components.movement.IsBoosted())
            middleEngine.SetBoostColor();
        else
            middleEngine.RestoreColor();
    }

    public void SetEnginesRestartMode(bool isOn)
    {
        float startSpeed = .5f;
        float lifeTime = 2f;

        if (isOn)
        {
            startSpeed = 20f;
            lifeTime = .2f;
        }

        // Start speed
        leftEngine.SetStartSpeed(startSpeed);
        rightEngine.SetStartSpeed(startSpeed);
        middleEngine.SetStartSpeed(startSpeed);

        // Life time
        leftEngine.SetLifeTime(lifeTime);
        rightEngine.SetLifeTime(lifeTime);
        middleEngine.SetLifeTime(lifeTime);
    }

    // Activate all engines owned by this ship
    public void TurnOnAllEngines()
    {
        middleEngine.Activate();
        leftEngine.Activate();
        rightEngine.Activate();
    }

    // Deactivate all engines owned by this ship
    public void TurnOffAllEngines()
    {
        middleEngine.Deactivate();
        leftEngine.Deactivate();
        rightEngine.Deactivate();
    }

    // Turn engines on or off based on player input received from PC
    public void ManageEngines(float horizontalInput, float verticalInput)
    {
        // Side engines
        if (horizontalInput > 0)
        {
            leftEngine.Activate();
            leftEngineOn = true;

            rightEngine.Deactivate();
            rightEngineOn = false;
        }
        else if (horizontalInput < 0)
        {
            rightEngine.Activate();
            rightEngineOn = true;

            leftEngine.Deactivate();
            leftEngineOn = false;
        }
        else
        {
            leftEngine.Deactivate();
            leftEngineOn = false;

            rightEngine.Deactivate();
            rightEngineOn = false;
        }

        // Front engines
        if (verticalInput > 0)
        {
            middleEngine.Activate();
            middleEngineOn = true;
        }
        else
        {
            middleEngine.Deactivate();
            middleEngineOn = false;
        }
    }

    #region Photon
    private bool middleEngineOn;
    private bool leftEngineOn;
    private bool rightEngineOn;

    // If writing, send engine status. Else, read engine status and apply.
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(middleEngineOn);
            stream.SendNext(leftEngineOn);
            stream.SendNext(rightEngineOn);
        }
        else
        {
            middleEngineOn = (bool)stream.ReceiveNext();
            leftEngineOn = (bool)stream.ReceiveNext();
            rightEngineOn = (bool)stream.ReceiveNext();

            if (middleEngineOn)
                middleEngine.Activate();
            else
                middleEngine.Deactivate();

            if (leftEngineOn)
                leftEngine.Activate();
            else
                leftEngine.Deactivate();

            if (rightEngineOn)
                rightEngine.Activate();
            else
                rightEngine.Deactivate();
        }
    }
    #endregion

    // Start coroutine to restore the system
    public void RestoreSystem()
    {
        StartCoroutine(C_FlickerEngines(3));
    }

    // Turns the boosted color on or off for all engines
    public void SetBoosted(bool boosted)
    {
        if (boosted)
        {
            middleEngine.SetBoostColor();
            leftEngine.SetBoostColor();
            rightEngine.SetBoostColor();
        }
        else
        {
            middleEngine.RestoreColor();
            leftEngine.RestoreColor();
            rightEngine.RestoreColor();
        }
    }

    // Flicker the engines several times, defined by times, for a random amount of ms, then tell the ship system it's ready to reboot
    private IEnumerator C_FlickerEngines(int times)
    {
        SetEnginesRestartMode(true);

        // Flicker the engines
        int counter = 0;
        while (counter < times)
        {
            TurnOnAllEngines();
            yield return new WaitForSeconds(Random.Range(0.3f, 0.6f));
            TurnOffAllEngines();
            yield return new WaitForSeconds(Random.Range(0.3f, 0.6f));

            counter++;
        }

        SetEnginesRestartMode(false);

        // Tell the ship's system it's ready to start up again
        parentShip.components.system.StartUp();
    }

    #region GetSet
    public void SetParentShip(Ship ship)
    {
        this.parentShip = ship;
    }
    #endregion

}
