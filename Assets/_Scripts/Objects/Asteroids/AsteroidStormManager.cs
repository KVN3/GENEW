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
    public SpawnPointManagerSettings SpawnPointManagerSettings;
    public LavaAsteroid[] redLavaAsteroidClasses;

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
        Assert.IsNotNull(redLavaAsteroidClasses);

        // Initializations
        spawnPointManager.settings = SpawnPointManagerSettings;
        spawnPoints = spawnPointManager.GenerateSpawnPoints(transform.position);
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
                int i = Random.Range(0, spawnPointManager.settings.rowLengthX);
                int j = Random.Range(0, spawnPointManager.settings.rowLengthZ);

                SpawnPoint sp = spawnPoints[i, j];

                if (sp.IsAvailable())
                {
                    SpawnAsteroid(sp);
                    sp.SetUnavailable();
                    StartCoroutine(UnlockSpawnPoint(sp, i, j));
                }
            }

            yield return new WaitForSeconds(0.1f);
        }
    }

    private IEnumerator UnlockSpawnPoint(SpawnPoint sp, int i, int j)
    {
        yield return new WaitForSeconds(3);
        sp.SetAvailable();
    }

    private void SpawnAsteroid(SpawnPoint spawnPoint)
    {
        Vector3 sp = spawnPoint.position;
        photonView.RPC("RPC_SpawnAsteroid", RpcTarget.AllViaServer, sp.x, sp.y, sp.z);
    }

    [PunRPC]
    public void RPC_SpawnAsteroid(float x, float y, float z)
    {
        //LavaAsteroid asteroidClass = redLavaAsteroidClasses[Random.Range(0, redLavaAsteroidClasses.Length)];
        string asteroidPath = SharedResources.GetPath("astre", Random.Range(0, 21));
        Asteroid asteroid = PhotonNetwork.Instantiate(asteroidPath, new Vector3(x, y, z), Quaternion.identity).GetComponent<Asteroid>();
        asteroid.transform.localScale = asteroid.transform.localScale * Random.Range(minAsteroidSize, maxAsteroidSize);
        asteroid.manager = this;
        asteroid.lifeTimeInSeconds = asteroidLifeTimeInSeconds;

        Floater floater = asteroid.gameObject.AddComponent<Floater>();
        floater.SetDirection(direction);
        floater.SetFloatSpeed(-16f);

        asteroids.Add(asteroid);
    }

    public void RemoveAsteroidFromObjectList(Asteroid asteroid)
    {
        asteroids.Remove(asteroid);
    }

}
