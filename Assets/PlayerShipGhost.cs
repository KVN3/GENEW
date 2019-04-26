using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShipGhost : MonoBehaviour
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
        StartCoroutine(startReplay());
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator startReplay()
    {
        yield return new WaitForSeconds(1.5f);
        while(i < posx.Length)
        {
            transform.rotation = rotations[i];
            transform.position = new Vector3(posx[i], posy[i], posz[i]);
            i++; 
            yield return null; 
            //yield return new WaitForEndOfFrame(); // makes replay slightly faster
        }
    }
}
