using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour
{
    public Node[] waypoints;
    public Transform player;
    public float moveSpeed = 5f;
    public float stoppingDistance = 0.1f; // Distancia mínima para considerar que llegó al nodo
    public LayerMask obstacleMask;

    private Node currentNode;
    private List<Node> currentPath;
    private int currentNodeIndex;
    private int currentWaypointIndex = 0;

    public bool isPatrolling = true; // Inicia patrullando
    public bool isAlert = false;

    public PathFinding pathFinding; // Sistema de PathFinding
    public Vector3 Position => transform.position;

    void Start()
    {
        // Inicialización de PathFinding
        pathFinding = PathFinding.Instace;

        // Configurar el nodo inicial más cercano al NPC
        currentNode = pathFinding.GetClosestNode(transform.position);
    }

void Update()
{
    // Detectar al jugador
    bool canSee = CanSeePlayer();

    // Si está alerta y no se ha generado el camino
    if (isAlert && (currentPath == null || currentPath.Count == 0))
    {
        GeneratePathToPlayer();
    }

    // Si está alerta, mover hacia el jugador
    if (isAlert && currentPath != null && currentPath.Count > 0)
    {
        MoveAlongPath();
    }
    // Si no está alerta, patrullar
    else if (!isAlert && isPatrolling)
    {
        Patrol();
    }
}
    public bool CanSeePlayer()
    {
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Raycast para verificar si el jugador está visible
        bool isObstructed = Physics.Raycast(transform.position, directionToPlayer, distanceToPlayer, obstacleMask);

        // Cambiar estados de alerta basados en el Raycast
        if (isObstructed)
        {
            Debug.DrawLine(transform.position, player.position, Color.yellow);
            isAlert = false;
            return false;
        }
        else
        {
            Debug.DrawLine(transform.position, player.position, Color.red);
            isAlert = true;
            return true;
        }
    }

    private void GeneratePathToPlayer()
{
    Debug.Log("Generando camino hacia el jugador...");
    currentPath = pathFinding.GetPath(transform.position, player.position);
    currentNodeIndex = 0; // Reiniciar el índice para recorrer el nuevo camino
}
public void MoveAlongPath()
{
    // Validar que el camino exista
    if (currentPath == null || currentPath.Count == 0)
    {
        Debug.Log("Camino no disponible.");
        return;
    }

    // Comprobar si el NPC ha llegado al último nodo
    if (currentNodeIndex >= currentPath.Count)
    {
        Debug.Log("NPC completó el camino.");
        currentPath = null; // Limpiar el camino
        return;
    }

    // Obtener el nodo objetivo actual
    Node targetNode = currentPath[currentNodeIndex];
    Vector3 direction = (targetNode.transform.position - transform.position).normalized;

    // Moverse hacia el nodo objetivo
    transform.position += direction * moveSpeed * Time.deltaTime;

    // Comprobar si llegó al nodo
    if (Vector3.Distance(transform.position, targetNode.transform.position) <= stoppingDistance)
    {
        currentNodeIndex++; // Avanzar al siguiente nodo
    }
}
private void Patrol()
{
    // Comprobar si hay waypoints definidos
    if (waypoints == null || waypoints.Length == 0) return;

    // Obtener el waypoint objetivo actual
    Transform targetWaypoint = waypoints[currentWaypointIndex].transform;

    // Calcular la dirección y mover el NPC hacia el waypoint
    Vector3 direction = (targetWaypoint.position - transform.position).normalized;
    transform.position += direction * moveSpeed * Time.deltaTime;

    // Verificar si el NPC ha alcanzado el waypoint
    if (Vector3.Distance(transform.position, targetWaypoint.position) <= stoppingDistance)
    {
        // Cambiar al siguiente waypoint de forma cíclica
        currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
    }
}
}


