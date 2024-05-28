using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCMove : TacticsMove
{
    // Objetivo actual del NPC
    GameObject target;

    void Start()
    {
        // Inicializa el NPC usando el método Init de TacticsMove
        Init(false);
    }

    void Update()
    {
        // Dibuja un rayo en la dirección hacia la que mira el NPC para depuración
        Debug.DrawRay(transform.position, transform.forward);

        // Si no es el turno del NPC, no hace nada
        if (!turn)
        {
            return;
        }

        // Si el NPC no está en movimiento
        if (!moving)
        {
            // Encuentra el objetivo más cercano
            FindNearestTarget();
            float distanceToTarget = Vector3.Distance(transform.position, target.transform.position);

            // Ataca al objetivo si está dentro del rango de ataque
            if (distanceToTarget <= characterStats.attackRange)
            {
                AttackTarget();
            }
            else
            {
                // Calcula el camino hacia el objetivo y se mueve hacia él
                CalculatePath();
                FindSelectableTiles();
                actualTargetTile.target = true;
                MoveToTile(actualTargetTile);
            }
        }
        else
        {
            // Mueve al NPC
            Move();
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

    // Ataca al objetivo
    void AttackTarget()
    {
        if (target != null)
        {
            TacticsMove targetMove = target.GetComponent<TacticsMove>();
            if (targetMove != null)
            {
                // Realiza el ataque
                CombatManager.Instance.Attack(this, targetMove);
                // Termina el turno del NPC
                EndTurn();
            }
        }
    }

    // Sobrescribe EndTurn para incluir lógica específica de NPC
    public override void EndTurn()
    {
        base.EndTurn();
        TurnManager.EndTurn();
    }
}
