using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCMove : TacticsMove
{
    GameObject target; // Objetivo actual del NPC

    void Start()
    {
        Init(false); // Inicializa el NPC usando el método Init de TacticsMove
    }

    void Update()
    {
        Debug.DrawRay(transform.position, transform.forward); // Dibuja un rayo en la dirección hacia la que mira el NPC para depuración

        if (!turn) // Si no es el turno del NPC, no hace nada
        {
            return;
        }

        if (!moving) // Si el NPC no está en movimiento
        {
            FindNearestTarget(); // Encuentra el objetivo más cercano
            CalculatePath(); // Calcula el camino hacia el objetivo
            FindSelectableTiles(); // Encuentra los tiles seleccionables
            actualTargetTile.target = true; // Marca el tile objetivo
        }
        else
        {
            Move(); // Mueve al NPC
        }
    }

    // Calcula el camino hacia el tile objetivo
    void CalculatePath()
    {
        Tile targetTile = GetTargetTile(target); // Obtiene el tile objetivo
        FindPath(targetTile); // Encuentra el camino hacia el tile objetivo
    }

    // Encuentra el objetivo más cercano
    void FindNearestTarget()
    {
        GameObject[] targets = GameObject.FindGameObjectsWithTag("Player"); // Encuentra todos los jugadores

        GameObject nearest = null;
        float distance = Mathf.Infinity;

        // Busca el jugador más cercano
        foreach (GameObject obj in targets)
        {
            float d = Vector3.Distance(transform.position, obj.transform.position);

            if (d < distance)
            {
                distance = d;
                nearest = obj;
            }
        }

        target = nearest; // Asigna el objetivo más cercano como el objetivo actual
    }
}
