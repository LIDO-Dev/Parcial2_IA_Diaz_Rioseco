using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathfindingChecker : MonoBehaviour
{
    [SerializeField] Transform _player;
    [SerializeField] Transform _target;

    public List<Node> path = new(); 

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            path = PathFinding.Instace.GetPath(_player.transform.position, _target.transform.position);
        }
    }

    private void OnDrawGizmos()
    {
        if(path.Count>0)
        {
            foreach(Node node in path)
            {
                Gizmos.DrawSphere(node.transform.position, 1);
            }
        }    
    }
}
