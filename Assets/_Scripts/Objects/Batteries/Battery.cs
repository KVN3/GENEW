using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Battery : MonoBehaviour
{
    public SpawnManager manager;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(WaitAndDestroy());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator WaitAndDestroy()
    {
        yield return new WaitForSeconds(20);
        if(manager != null)
            manager.RemoveBatteryFromObjectList(this);
        Destroy(gameObject);
    }
}
