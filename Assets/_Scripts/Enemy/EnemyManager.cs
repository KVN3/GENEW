using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class EnemyManager : MonoBehaviour
{
    public EnergyBall[] enemyClasses;
    public int desiredAliveEnemyCount;

    // Spawn Points
    protected List<LocalSpawnPoint> spawnPoints;
    protected List<LocalSpawnPoint> usedSpawnPoints;
    protected LocalSpawnPoint sp;

    protected PlayerShip[] players;
    protected List<EnergyBall> enemies = new List<EnergyBall>();

    protected PhotonView photonView;

    [SerializeField]
    protected int viewId;

    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
        photonView.ViewID = viewId;
    }

    private void Start()
    {
        Assert.IsFalse(enemyClasses.Length == 0);

        usedSpawnPoints = new List<LocalSpawnPoint>();

        StartCoroutine(SpawnLoop());
    }

    private IEnumerator SpawnLoop()
    {
        while (true)
        {
            if (enemies.Count < desiredAliveEnemyCount)
                SpawnEnemy();

            yield return new WaitForSeconds(3);
        }
    }

    protected virtual void SpawnEnemy()
    {
        if (spawnPoints.Count == 0)
        {
            foreach (LocalSpawnPoint spawnPoint in usedSpawnPoints)
                spawnPoints.Add(spawnPoint);

            usedSpawnPoints = new List<LocalSpawnPoint>();
        }

        sp = spawnPoints[Random.Range(0, spawnPoints.Count)];
        spawnPoints.Remove(sp);
        usedSpawnPoints.Add(sp);
    }

    public void SetPlayers(PlayerShip[] players)
    {
        this.players = players;
    }

    public void SetSpawnPoints(List<LocalSpawnPoint> spawnPoints)
    {
        this.spawnPoints = spawnPoints;
    }

    public void RemoveFromAliveEnemies(EnergyBall enemy)
    {
        enemies.Remove(enemy);
    }

    //public void RefreshTargets(List<int> removedShipIndexes)
    //{
    //    foreach (int index in removedShipIndexes)
    //    {
    //        players.RemoveAt(index);
    //    }
    //}
}
