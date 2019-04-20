using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmokeScreen : MonoBehaviour
{
    public float duration = 5f;

    void Start()
    {
        StartCoroutine(WaitAndDestroy());
    }

    IEnumerator WaitAndDestroy()
    {
        yield return new WaitForSeconds(duration);
        Destroy(gameObject);
    }
}
