using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class MovingObstacle : MonoBehaviour
{
    public Direction direction;

    public float delta = 1.5f;
    public float speed = 2.0f;
    public bool swapAxis;

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        Vector3 v = startPos;

        if (direction == Direction.Z)
            v.z += delta * Mathf.Sin(Time.time * speed);
        else if (direction == Direction.X)
            v.x += delta * Mathf.Sin(Time.time * speed);
        else if (direction == Direction.Y)
            v.y += delta * Mathf.Sin(Time.time * speed);

        transform.position = v;
    }

    private IEnumerator RandomizeRange()
    {
        while (true)
        {
            yield return new WaitForSeconds(10);
            delta = Random.Range(20, delta);

            if (swapAxis)
            {
                if (direction == Direction.X)
                    direction = Direction.Z;
                else
                    direction = Direction.X;
            }
                
        }
    }
}
