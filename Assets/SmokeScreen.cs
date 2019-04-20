using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmokeScreen : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(WaitAndDestroy());
    }

    IEnumerator WaitAndDestroy()
    {
        yield return new WaitForSeconds(5);
        Destroy(gameObject);
    }
}
