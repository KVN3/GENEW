using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

[System.Serializable]
public struct SpawnPointManagerSettings
{
    public int rowLengthX, rowLengthZ;
    public int spacing;
}

public class SpawnPointManager : MonoBehaviour
{
    public List<LocalSpawnPoint> chaserSpawnPoints;
    public List<LocalSpawnPoint> movingSpawnPoints;

    void Start()
    {
        //Assert.IsTrue(settings.rowLengthX > 0, "rowLengthX = 0");
        //Assert.IsTrue(settings.rowLengthZ > 0, "rowLengthZ = 0");
        //Assert.IsTrue(settings.spacing > 0, "spacing = 0");
    }

    public SpawnPoint[,] GenerateSpawnPoints(Vector3 startPos, SpawnPointManagerSettings settings)
    {
        SpawnPoint[,] tempSpawnPoints = new SpawnPoint[settings.rowLengthX, settings.rowLengthZ];

        for (int i = 0; i < settings.rowLengthX; i++)
        {
            for (int j = 0; j < settings.rowLengthZ; j++)
            {
                SpawnPoint sp = ScriptableObject.CreateInstance<SpawnPoint>();
                sp.position = new Vector3(startPos.x + (i * settings.spacing), startPos.y, startPos.z + (j * settings.spacing));
                tempSpawnPoints[i, j] = sp;
            }
        }

        return tempSpawnPoints;
    }
}
