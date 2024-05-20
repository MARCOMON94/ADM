using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCMove : TacticsMove
{
    GameObject target;

    // Método Start que se llama al inicio
    void Start()
    {
        Init(); // Llama al método Init de TacticsMove para inicializar
    }

    // Método Update que se llama una vez por frame
    void Update()
    {
        Debug.DrawRay(transform.position, transform.forward);

        if (!turn)
        {
            return; // Si no es el turno del NPC, no hace nada
        }

        if (!moving)
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

    // Método para calcular el camino hacia el tile objetivo
    void CalculatePath()
    {
        Tile targetTile = GetTargetTile(target); // Obtiene el tile objetivo
        FindPath(targetTile); // Encuentra el camino hacia el tile objetivo
    }

    // Método para encontrar el objetivo más cercano
    void FindNearestTarget()
    {
        GameObject[] targets = GameObject.FindGameObjectsWithTag("Player");

        GameObject nearest = null;
        float distance = Mathf.Infinity;

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

