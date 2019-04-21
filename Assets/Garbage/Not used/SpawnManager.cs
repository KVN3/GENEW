using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;


[System.Serializable]
public struct ObjectClasses
{
    public Asteroid[] blueElectricAsteroidClasses;
    public Asteroid[] redLavaAsteroidClasses;

    public Battery[] batteryClasses;
}

[System.Serializable]
public struct SpawnSettings
{
    public int difficulty;
    public int desiredAsteroidCount;
}

public class SpawnManager : MonoBehaviour
{
    public ObjectClasses objectClasses = new ObjectClasses();
    public SpawnSettings settings = new SpawnSettings();
    public AttachableScripts scripts = new AttachableScripts();

    // Currently spawned items
    private List<Asteroid> asteroids;
    private List<Battery> batteries;

    // playerShip ship object
    public PlayerShip PlayerShip;

    void Start()
    {
        // Assertions
        Assert.IsNotNull(objectClasses.blueElectricAsteroidClasses);
        Assert.IsNotNull(objectClasses.redLavaAsteroidClasses);
        Assert.IsNotNull(objectClasses.batteryClasses);

        // Initializations
        asteroids = new List<Asteroid>();
        batteries = new List<Battery>();

        // Coroutines
        StartCoroutine(SpawnLoop());
    }

    private IEnumerator SpawnLoop()
    {
        // Initial wait before spawning
        yield return new WaitForSeconds(3);

        // Spawning loop
        while (true)
        {
            if (asteroids.Count < settings.desiredAsteroidCount)
            {
                int i = 0;

                // Asteroid spawn loop
                while (i < (2 + settings.difficulty))
                {
                    SpawnAsteroid();
                    i++;
                }

                // Battery spawn (20% chance)
                bool shouldSpawnBattery = Random.Range(0, 4) == 1;
                if (shouldSpawnBattery)
                    SpawnBattery();
            }

            yield return new WaitForSeconds(GetLoopWaitTime());
        }
    }

    private void SpawnBattery()
    {
        Battery batteryClass = objectClasses.batteryClasses[Random.Range(0, objectClasses.batteryClasses.Length)];
        Vector3 pos = CalculateSpawnPosition();

        Battery battery = Instantiate(batteryClass, pos, Quaternion.identity);
        battery.manager = this;

        // Instantiate empty game object with floater script and attach to battery game object
        //Floater floater = Instantiate(scripts.floaterScript, pos, Quaternion.identity);
        battery.gameObject.AddComponent<Floater>();
        //floater.transform.parent = battery.transform;
        //floater.masterObject = battery.transform.parent.gameObject;
        //battery.floaterScript = scripts.floaterScript;
        batteries.Add(battery);

    }

    private void SpawnAsteroid()
    {
        Asteroid asteroidClass;

        switch (settings.difficulty)
        {
            case 0:
                asteroidClass = objectClasses.blueElectricAsteroidClasses[Random.Range(0, objectClasses.blueElectricAsteroidClasses.Length)];
                break;
            case 1:
                asteroidClass = objectClasses.redLavaAsteroidClasses[Random.Range(0, objectClasses.redLavaAsteroidClasses.Length)];
                break;
            default:
                asteroidClass = objectClasses.blueElectricAsteroidClasses[Random.Range(0, objectClasses.blueElectricAsteroidClasses.Length)];
                break;
        }


        Vector3 pos = CalculateSpawnPosition();

        Asteroid asteroid = Instantiate(asteroidClass, pos, Quaternion.identity);
        asteroid.transform.localScale = asteroid.transform.localScale * Random.Range(0.2f, 3f);
        //asteroid.manager = this;
        asteroids.Add(asteroid);
    }

    public Vector3 CalculateSpawnPosition()
    {
        Vector3 pos = new Vector3();

        // Based on playerShip pos
        //float playerX = playerShip.transform.position.x;
        //pos.x = Random.Range(playerX - 30f, playerX + 30f);

        // Set bounds
        pos.x = Random.Range(-35f, 35f);

        pos.z = PlayerShip.transform.position.z - Random.Range(200, 300);
        pos.y = 0;

        return pos;
    }

    public float GetLoopWaitTime()
    {
        float waitTimeInSeconds = 2f - (settings.difficulty * 0.5f);

        if (waitTimeInSeconds < 0.5f)
        {
            waitTimeInSeconds = 0.5f;
            Debug.Log($"Difficulty level ({settings.difficulty.ToString()}) too high: lowest wait time 500ms set.");
        }

        return waitTimeInSeconds;

    }

    public void RemoveAsteroidFromObjectList(Asteroid asteroid)
    {
        asteroids.Remove(asteroid);
    }

    public void RemoveBatteryFromObjectList(Battery battery)
    {
        batteries.Remove(battery);
    }
}
