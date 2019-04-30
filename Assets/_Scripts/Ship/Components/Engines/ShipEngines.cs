using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class ShipEngines : ShipComponent
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
        if (parentShip.movement.IsBoosted())
        {
            middleEngine.SetBoostColor();
        }
        else
        {
            middleEngine.RestoreColor();
        }
    }

    private IEnumerator FlickerEngines(int times)
    {
        SetEnginesRestartMode(true);

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
        parentShip.components.system.StartUp();
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

    public void TurnOnAllEngines()
    {
        middleEngine.Activate();
        leftEngine.Activate();
        rightEngine.Activate();
    }

    public void TurnOffAllEngines()
    {
        middleEngine.Deactivate();
        leftEngine.Deactivate();
        rightEngine.Deactivate();
    }

    public void ManageEngines(float horizontalInput)
    {
        if (horizontalInput > 0)
        {
            leftEngine.Activate();
            rightEngine.Deactivate();
        }
        else if (horizontalInput < 0)
        {
            rightEngine.Activate();
            leftEngine.Deactivate();
        }
        else
        {
            leftEngine.Deactivate();
            rightEngine.Deactivate();
        }
    }

    public void RestoreSystem()
    {
        StartCoroutine(FlickerEngines(3));
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


    #region GetSet
    public void SetParentShip(Ship ship)
    {
        this.parentShip = ship;
    }
    #endregion

}
