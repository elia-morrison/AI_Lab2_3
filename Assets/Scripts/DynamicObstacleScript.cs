using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicObstacleScript : MonoBehaviour
{
    public Vector3 PointA;
    public Vector3 PointB;
    public float Speed = 16f;
    public bool Reverse = false;
    private readonly float threshold = 1e-4f;

    private void FixedUpdate()
    {
        Vector3 current = transform.position;
        Vector3 target = Reverse ? PointB : PointA;
        transform.position = Vector3.MoveTowards(current, target, Speed * Time.fixedDeltaTime);

        if (Vector3.Distance(transform.position, target) < threshold)
            Reverse = !Reverse;
    }
}
