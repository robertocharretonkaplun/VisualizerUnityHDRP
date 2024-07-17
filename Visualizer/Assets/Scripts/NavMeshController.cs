using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// En esta clase se maneja el movimiento del agente en NavMesh para poder desplazarse en el nivel.
/// </summary>

public class NavMeshController : MonoBehaviour
{
    /// <summary>
    /// La c�mara principal nos ayudar� para obtener la posici�n del clic del usuario.
    /// </summary>
    public Camera cam;

    /// <summary>
    /// NavMesh Agent es el objeto que se mover� hacia la posici�n donde se dio el clic por el usuario.
    /// </summary>
    public UnityEngine.AI.NavMeshAgent agent;

    /// <summary>
    /// Este método comprueba si el usuario ha hecho clic y, de ser así, calcula un destino
    /// para el agente NavMesh basado en la posición del clic en el nivel.
    /// </summary>
    void Update()
    {
        // Comprueba si se ha presionado el botón izquierdo del ratón (botón 0)
        if (Input.GetMouseButtonDown(0))
        {
            // En esta parte toma la posición del ratón en la pantalla y proyecta un rayo desde la cámara
            //  hacia esa posición en el mundo (El rayo es una línea invisible que se utiliza para detectar
            //  colisiones con objetos en la escena.)
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            // Almacenará información sobre la colisión, si la hay. RaycastHit es una estructura que contiene
            //  datos sobre el punto de impacto, la distancia desde el origen del rayo y donde el collider.
            RaycastHit hit;
            // Si el rayo colisiona con algún objeto en la escena. La información sobre la colisión se almacena
            // en la variable hit. El out keyword indica que hit será un parámetro de salida que recibirá el valor
            // calculado dentro del método.
            if (Physics.Raycast(ray, out hit))
            {
                // Si el rayo colisiona con un objeto, este método establece el destino del NavMeshAgent en el
                // punto de colisión (hit.point). El agente se moverá automáticamente hacia esa posición utilizando
                // la navegación proporcionada por el sistema de NavMesh de Unity.
                agent.SetDestination(hit.point);
            }
        }
    }
}
