using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    [SerializeField] private List<Node> _neighbours = new();
    public List<Node> Neighbours { get { return _neighbours; } }

    [SerializeField] private float searchingRange;

    [SerializeField] private LayerMask _layerMask;

    public Node previousNode;

    public float heuristic = 99999;
    void Awake()
    {
        heuristic = 99999;
        var colliders = Physics.
            OverlapSphere(transform.position, searchingRange, LayerMask.GetMask("Node", "FlyingNode"));

        foreach (var collider in colliders)
        {
            var node = collider.GetComponent<Node>();

            if (node == null
                || node == this
                || _neighbours.Contains(node)
                || !PathFinding.OnSight(transform.position, node.transform.position)) continue;

            _neighbours.Add(node);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, searchingRange);
    }
}
