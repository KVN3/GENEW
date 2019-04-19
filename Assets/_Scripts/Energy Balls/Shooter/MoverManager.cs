using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoverManager : EnemyManager
{
    protected override void SpawnEnemy()
    {
        base.SpawnEnemy();

        RandomMover moverClass = enemyClasses[Random.Range(0, enemyClasses.Length)] as RandomMover;
        moverClass.xMax = sp.xMax;
        moverClass.xMin = sp.xMin;
        moverClass.zMax = sp.zMax;
        moverClass.zMin = sp.zMin;
        moverClass.maxSpeed = sp.maxSpeed;
        RandomMover mover = Instantiate(moverClass, sp.transform.position, Quaternion.identity);

        

        mover.shooterModule.SetTargets(players);
        mover.SetManager(this);
        enemies.Add(mover);
    }
}
