using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NPCMove : TacticsMove
{
    GameObject target;

    void Start()
    {
        Init(false);
        UpdateHealthUI();
    }

    void Update()
    {
        Debug.DrawRay(transform.position, transform.forward);

        if (!turn)
        {
            return;
        }

        if (!moving)
        {
            FindNearestTarget();
            float distanceToTarget = Vector3.Distance(transform.position, target.transform.position);

            if (distanceToTarget <= characterStats.attackRange)
            {
                AttackTarget();
            }
            else
            {
                CalculatePath();
                FindSelectableTiles();
                actualTargetTile.target = true;
                MoveToTile(actualTargetTile);
            }
        }
        else
        {
            Move();
        }
    }

    void CalculatePath()
    {
        Tile targetTile = GetTargetTile(target);
        FindPath(targetTile);
    }

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

        target = nearest;
    }

    void AttackTarget()
{
    if (target != null)
    {
        TacticsMove targetMove = target.GetComponent<TacticsMove>();
        if (targetMove != null)
        {
            float distanceToTarget = Vector3.Distance(new Vector3(transform.position.x, 0, transform.position.z),
                                                      new Vector3(targetMove.transform.position.x, 0, targetMove.transform.position.z));
            Debug.Log($"Distancia al objetivo {targetMove.name}: {distanceToTarget}, rango de ataque: {characterStats.attackRange}");

            if (distanceToTarget <= characterStats.attackRange + 0.05f) // Añadimos un pequeño margen para problemas de precisión
            {
                if (characterStats.attackType == AttackType.Normal)
                {
                    Debug.Log($"Realizando ataque normal a {targetMove.name}");
                    CombatManager.Instance.Attack(this, targetMove);
                }
                else if (characterStats.attackType == AttackType.Pierce)
                {
                    Debug.Log($"Realizando ataque de penetración a {targetMove.name}");
                    CombatManager.Instance.AttackWithPierce(this, targetMove);
                }
                EndTurn();
            }
            else
            {
                Debug.Log($"Objetivo {targetMove.name} fuera de rango");
            }
        }
        else
        {
            Debug.Log("Componente TacticsMove no encontrado en el objetivo");
        }
    }
    else
    {
        Debug.Log("No se ha encontrado objetivo para atacar");
    }
}






    public override void EndTurn()
    {
        base.EndTurn();
        TurnManager.EndTurn();
    }

    public override void UpdateHealthUI()
{
    int characterIndex = GetCharacterIndex();
    if (characterIndex != -1)
    {
        Debug.Log($"Actualizando salud de {characterStats.name} en el índice {characterIndex} con salud {characterStats.health}");
        UIManager.Instance.UpdateCharacterHealth(characterIndex, characterStats.health);
    }
}

    private int GetCharacterIndex()
    {
        if (characterStats.name == "Avelino") return 2;
        if (characterStats.name == "Pepa") return 3;
        if (characterStats.name == "Paco") return 0;
        if (characterStats.name == "Loli") return 1;
        
        Debug.LogWarning($"Nombre de personaje {characterStats.name} no asignado a ningún índice");
        return -1;
    }
}
