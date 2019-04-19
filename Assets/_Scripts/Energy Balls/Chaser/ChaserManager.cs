using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaserManager : EnemyManager
{
    protected override void SpawnEnemy()
    {
        base.SpawnEnemy();

        Chaser enemyClass = enemyClasses[Random.Range(0, enemyClasses.Length)] as Chaser;
        Chaser chaser = Instantiate(enemyClass, sp.transform.position, Quaternion.identity);
        chaser.SetTargets(players);
        chaser.SetManager(this);
        enemies.Add(chaser);
    }
}
