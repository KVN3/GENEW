using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class AsteroidStormManager : MonoBehaviour
{
    public int desiredAsteroidCount;
    public Direction direction;
    public float minAsteroidSize;
    public float maxAsteroidSize;

    public SpawnPointManager spawnPointManager;
    public SpawnPointManagerSettings spawnpointSettings;
    public Asteroid[] asteroidClasses;

    // Currently spawned items
    private List<Asteroid> asteroids;
    private SpawnPoint[,] spawnPoints;

    private PhotonView photonView;
    [SerializeField]
    private int viewId;

    [SerializeField]
    private int asteroidLifeTimeInSeconds;

    // Start is called before the first frame update
    void Start()
    {
        // Assertions
        Assert.IsNotNull(asteroidClasses);

        // Initializations
        //spawnPointManager.settings = SpawnPointManagerSettings;
        spawnPoints = spawnPointManager.GenerateSpawnPoints(transform.position, spawnpointSettings);
        asteroids = new List<Asteroid>();

        photonView = GetComponent<PhotonView>();
        photonView.ViewID = viewId;

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
            if (asteroids.Count < desiredAsteroidCount)
            {
                int i = Random.Range(0, spawnpointSettings.rowLengthX);
                int j = Random.Range(0, spawnpointSettings.rowLengthZ);

                SpawnPoint sp = spawnPoints[i, j];

                if (sp.IsAvailable())
                {
                    SpawnAsteroid(sp);
                    sp.SetUnavailable();
                    StartCoroutine(C_UnlockSP(sp, i, j));
                }
            }

            yield return new WaitForSeconds(0.1f);
        }
    }

    private IEnumerator C_UnlockSP(SpawnPoint sp, int i, int j)
    {
        yield return new WaitForSeconds(3);
        sp.SetAvailable();
    }

    private void SpawnAsteroid(SpawnPoint spawnPoint)
    {
        string asteroidPath = SharedResources.GetPath("astre", Random.Range(0, 3));
        Asteroid asteroid = PhotonNetwork.Instantiate(asteroidPath, spawnPoint.position, Quaternion.identity).GetComponent<Asteroid>();
        asteroid.transform.localScale = asteroid.transform.localScale * Random.Range(minAsteroidSize, maxAsteroidSize);
        asteroid.manager = this;
        asteroid.lifeTimeInSeconds = asteroidLifeTimeInSeconds;
        asteroids.Add(asteroid);


        // Has to be same for all clients
        //Vector3 sp = spawnPoint.position;
        //string asteroidPath = SharedResources.GetPath("astre", Random.Range(0, 21));

        // On all clients
        //photonView.RPC("RPC_SpawnAsteroid", RpcTarget.AllViaServer, sp, asteroidPath);
    }

    [PunRPC]
    public void RPC_SpawnAsteroid(Vector3 sp, string asteroidPath)
    {
        Asteroid asteroidClass = asteroidClasses[Random.Range(0, asteroidClasses.Length)];
        Asteroid asteroid = Instantiate(asteroidClass, sp, Quaternion.identity);

        //Asteroid asteroid = PhotonNetwork.Instantiate(asteroidPath, sp, Quaternion.identity).GetComponent<Asteroid>();
        asteroid.transform.localScale = asteroid.transform.localScale * Random.Range(minAsteroidSize, maxAsteroidSize);
        asteroid.manager = this;
        asteroid.lifeTimeInSeconds = asteroidLifeTimeInSeconds;

        //Floater floater = asteroid.gameObject.AddComponent<Floater>();
        //floater.SetDirection(direction);
        //floater.SetFloatSpeed(-16f);

        asteroids.Add(asteroid);
    }

    public void RemoveAsteroidFromObjectList(Asteroid asteroid)
    {
        asteroids.Remove(asteroid);
    }

}
