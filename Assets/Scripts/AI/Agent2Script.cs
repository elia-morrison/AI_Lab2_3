using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Agent2Script : MonoBehaviour
{
    private NavMeshAgent agent;
    private GameObject targetPointer;
    private Vector3? target;
    public float UpdateInterval = 0.1f;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        InvokeRepeating(nameof(SetDestination), 0.0f, 1.0f);
    }

    void Update()
    {

        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var hit))
            {
                target = hit.point;
            }
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            GameObject target = GameObject.FindGameObjectWithTag("Target");
            if (target != null)
            {
                Physics.Raycast(target.transform.position, Vector3.down * 100, out var hit);
                this.target = hit.point;
            }
        }
    }

    void DisplayerTargetPointer(Vector3 target)
    {
        if (targetPointer != null)
        {
            Destroy(targetPointer);
        }
        targetPointer = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        targetPointer.transform.position = target;
        targetPointer.transform.localScale = new Vector3(2.0f, 2.0f, 2.0f);
        Material newMat = Resources.Load("Materials/TargetMat", typeof(Material)) as Material;
        targetPointer.GetComponent<Renderer>().material = newMat;
    }

    

    void SetDestination()
    {
        if (target.HasValue)
        {

            NavMeshPath path = new();
            DisplayerTargetPointer(target.Value);
            agent.CalculatePath(target.Value, path);
            agent.SetPath(path);
        }
    }
}
