using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class AgentScript : MonoBehaviour
{
    private NavMeshAgent agent;
    private Coroutine walking;
    private readonly float threshold = 5.0f;
    private GameObject targetPointer;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var hit))
            {
                // agent.SetDestination(hit.point);
                TryGoToTarget(hit.point);
            }
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            GameObject target = GameObject.FindGameObjectWithTag("Target");
            if (target != null)
            {
                // agent.SetDestination(target.transform.position);
                TryGoToTarget(target.transform.position);

            }
        }
    }

    IEnumerator Walk(Queue<Vector3> path)
    {
        agent.ResetPath();
        while (path.Count > 0)
        {
            Vector3 target = path.Dequeue();
            NavMeshPath navMeshPath = new();

            // Wait until the nest MeshPath is available
            while (!(agent.CalculatePath(target, navMeshPath) && navMeshPath.status == NavMeshPathStatus.PathComplete))
                yield return new WaitForFixedUpdate();

            agent.SetPath(navMeshPath);

            // Do nothing while the agent is moving to the target point
            while (agent.remainingDistance > threshold)
                yield return new WaitForFixedUpdate();
        }

        walking = null;
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

    void TryGoToTarget(Vector3 target)
    {
        DisplayerTargetPointer(target);
        var owner = agent.navMeshOwner as Component;
        owner.TryGetComponent(out AreaScript ownerArea);

        // Index of the NavMesh which the agent is currently on
        int currentArea = ownerArea.areaIndex;
        NavMeshPath path = new();

        // If we are in the same area as target point, then just move to it
        if (agent.CalculatePath(target, path) && path.status == NavMeshPathStatus.PathComplete)
            agent.SetPath(path);
        else
        {
            foreach (var area in AreaScript.areas.Values)
            {
                // We'll need a point which is certainly on the mesh
                Vector3 pointInArea = area.neighbors[0].From;

                // Check if target is in the area
                NavMesh.CalculatePath(pointInArea, target, NavMesh.AllAreas, path);

                // Target point is not in this area, skip
                if (path.status == NavMeshPathStatus.PathInvalid)
                    continue;

                // We have found the index of the area, on which the target point is located
                var pathToTarget = AreaScript.FindPath(currentArea, area.areaIndex);
                if (pathToTarget.Count == 0)
                    continue;

                // Add the last point of the path (target)
                pathToTarget.Enqueue(target);

                if (walking != null)
                {
                    StopCoroutine(walking);
                    walking = null;
                }

                StartCoroutine(Walk(pathToTarget));
                return;
            }
        }
    }
}
