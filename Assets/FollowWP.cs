using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class FollowWP : MonoBehaviour
{
    public GameObject[] waypoints;
    private int currentWP = 0;

    public float speed = 10.0f;
    public float rotSpeed = 10.0f;
    public float waypointThreshold = 3.0f;

    private Rigidbody rb;

    void Start()
    {
        // Obtener el componente Rigidbody
        rb = GetComponent<Rigidbody>();
        // Asegurar que el Rigidbody esté en modo cinemático para evitar conflictos de física no deseados
        rb.isKinematic = false;
    }

    void FixedUpdate()
    {
        // Verificar si estamos cerca del waypoint actual
        if (Vector3.Distance(transform.position, waypoints[currentWP].transform.position) < waypointThreshold)
        {
            currentWP++;  // Cambiar al siguiente waypoint

            // Si hemos alcanzado el último waypoint, reiniciamos el índice
            if (currentWP >= waypoints.Length)
                currentWP = 0;
        }

        // Calcular la rotación hacia el siguiente waypoint
        Vector3 direction = (waypoints[currentWP].transform.position - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(direction);

        // Aplicar rotación usando Rigidbody
        rb.rotation = Quaternion.Slerp(rb.rotation, targetRotation, rotSpeed * Time.deltaTime);

        // Moverse hacia adelante en la dirección actual
        rb.MovePosition(transform.position + transform.forward * speed * Time.deltaTime);
    }

    // Método para visualizar la ruta con Gizmos en el editor
    private void OnDrawGizmos()
    {
        if (waypoints == null || waypoints.Length < 2) return;

        Gizmos.color = Color.cyan;
        for (int i = 0; i < waypoints.Length; i++)
        {
            // Dibujar esferas en cada waypoint
            Gizmos.DrawSphere(waypoints[i].transform.position, 0.5f);

            // Dibujar líneas entre los waypoints
            if (i < waypoints.Length - 1)
                Gizmos.DrawLine(waypoints[i].transform.position, waypoints[i + 1].transform.position);
            else
                Gizmos.DrawLine(waypoints[i].transform.position, waypoints[0].transform.position); // Línea final hacia el primer waypoint
        }
    }
}
