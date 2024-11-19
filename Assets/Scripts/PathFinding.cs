using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PathFinding : MonoBehaviour
{
    public static PathFinding Instace;

    private void Awake()
    {
        if (Instace != null)
        { 
            Destroy(gameObject);
            return;
        }

        Instace = this;
    }

    //A*
    //Proxima Clase Theta*
    public List<Node> GetPath(Vector3 startPosition, Vector3 endPosition)
    {
        var startNode = GetClosestNode(startPosition);
        startNode.heuristic = 0;
        var endNode = GetClosestNode(endPosition);

        var openNode = new PriorityQueue<Node>();
        var closeNode = new HashSet<Node>();

        openNode.Enqueue(startNode, startNode.heuristic);

        var watchdog = 5000;
        while(openNode.Count > 0 && watchdog>0)
        {
            var actualNode = openNode.Dequeue();
            if (actualNode == endNode) break;

            Debug.Log(actualNode.name);

            watchdog--;
            Debug.Log(watchdog);

            foreach (var neighbour in actualNode.Neighbours)
            {
                if (closeNode.Contains(neighbour)) continue;

                var heuristic = actualNode.heuristic +
                    Vector3.Distance(actualNode.transform.position, neighbour.transform.position) +
                    Vector3.Distance(endNode.transform.position, neighbour.transform.position);

                var heuristicSameDistance = actualNode.heuristic +
                    1 +
                    Vector3.Distance(endNode.transform.position, neighbour.transform.position);

                if (neighbour.heuristic > heuristic)
                {
                    neighbour.heuristic = heuristic;
                    neighbour.previousNode = actualNode;
                }

                openNode.Enqueue(neighbour, neighbour.heuristic);
            }

            closeNode.Add(actualNode);
        }

        var path = new List<Node>();
        var pathNode = endNode;
        path.Add(pathNode);
        while (pathNode != startNode)
        {
            pathNode = pathNode.previousNode;
            path.Add(pathNode);
        }

        path.Reverse();
        return path;

    }

    public Node GetClosestNode(Vector3 point)
    {
        var searchingRange = 2;
        var colliders = Physics.
            OverlapSphere(point, searchingRange, LayerMask.GetMask("Node", "FlyingNode"));

        colliders = GetOnSightNode(point, colliders).ToArray();

        while(colliders.Length == 0)
        {
            searchingRange += 2;
            colliders = Physics.
                OverlapSphere(point, searchingRange, LayerMask.GetMask("Node", "FlyingNode"));

            colliders = GetOnSightNode(point, colliders).ToArray();
        }

        //Muestra de Linq
        //var closest = colliders.OrderBy(x => Vector3.Distance(point, x.transform.position))
        //    .First();

        var closestNode = colliders[0];
        var minDistance = Vector3.Distance(point, closestNode.transform.position);

        for (int i = 1; i < colliders.Length; i++)
        {
            if (Vector3.Distance(colliders[0].transform.position, point) < minDistance)
            {
                closestNode = colliders[i];
                minDistance = Vector3.Distance(point, closestNode.transform.position);
            }
        }

        return closestNode.GetComponent<Node>();
    }

    public List<Collider> GetOnSightNode(Vector3 from, Collider[] colliders)
    {
        var onSightNode = new List<Collider>();

        foreach(var collider in colliders)
        {
            if (OnSight(from, collider.transform.position))
                onSightNode.Add(collider);
        }
        return onSightNode;
    }

    public static bool OnSight(Vector3 from, Vector3 to)
    {
        var dir = to - from;
        return !Physics.Raycast(from, dir, dir.magnitude, LayerMask.GetMask("Wall"));
    }
}

