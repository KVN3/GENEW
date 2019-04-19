using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : ScriptableObject
{
    public Vector3 position;
    private bool isAvailable = true;

    public void SetUnavailable()
    {
        this.isAvailable = false;
    }

    public void SetAvailable()
    {
        this.isAvailable = true;
    }

    public bool IsAvailable()
    {
        return isAvailable;
    }

    
}
