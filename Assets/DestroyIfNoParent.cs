using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyIfNoParent : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(CheckParentLoop());

    }

    private IEnumerator CheckParentLoop()
    {
        while (true)
        {
            if (transform.parent == null)
                Destroy(gameObject);

            yield return new WaitForSeconds(2);
        }
    }
}
