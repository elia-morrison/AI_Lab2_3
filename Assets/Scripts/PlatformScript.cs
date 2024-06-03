using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformScript : MonoBehaviour
{
    [SerializeField] float rotationSpeed = 30.0f;
    private Vector3 rotationCenter;
    // Start is called before the first frame update
    void Start()
    {
        rotationCenter = transform.position + 10 * Vector3.back;
    }

    // Update is called once per frame
    void Update()
    {
        transform.RotateAround(rotationCenter, Vector3.up, Time.deltaTime * rotationSpeed);
    }
}
