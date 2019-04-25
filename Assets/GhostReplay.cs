using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostReplay : MonoBehaviour
{
    float[] posx;
    float[] posy;
    float[] posz;
    Quaternion[] rotations;

    int i = 0;

    // Start is called before the first frame update
    void Start()
    {
        posx = PlayerPrefsX.GetFloatArray("ReplayX");
        posy = PlayerPrefsX.GetFloatArray("ReplayY");
        posz = PlayerPrefsX.GetFloatArray("ReplayZ");
        rotations = PlayerPrefsX.GetQuaternionArray("ReplayQ");
    }

    // Update is called once per frame
    void Update()
    {
        // Only change position when race started and array is not done
        if (i < posx.Length) //RaceManager.raceStarted && 
        {
            transform.rotation = rotations[i];
            transform.position = new Vector3(posx[i], posy[i], posz[i]);
            i++;
        }
    }
}
