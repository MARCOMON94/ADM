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
            // Llama a FindSelectableTiles con ignoreOccupied = true cuando en modo ataque
            FindSelectableTiles(true);
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
                float heightDifference = Mathf.Abs(targetMove.transform.position.y - transform.position.y);

                if (distanceToTarget <= characterStats.attackRange + 0.05f && 
                    (characterStats.heightAttack || heightDifference <= characterStats.jumpHeight))
                {
                    if (characterStats.attackType == AttackType.Normal)
                    {
                        CombatManager.Instance.Attack(this, targetMove);
                    }
                    else if (characterStats.attackType == AttackType.Pierce)
                    {
                        CombatManager.Instance.AttackWithPierce(this, targetMove);
                    }
                    EndTurn();
                }
                else
                {
                    Debug.Log($"Target {targetMove.name} out of attack range or height difference is too great.");
                    EndTurn(); // Asegurarse de que el turno termine si no se puede atacar
                }
            }
            else
            {
                Debug.Log("TacticsMove component not found on target.");
                EndTurn();
            }
        }
        else
        {
            Debug.Log("No target found to attack.");
            EndTurn();
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
        if (characterStats.name == "Nekomaru") return 0;
        if (characterStats.name == "Aya") return 1;
        if (characterStats.name == "Umi") return 2;
        if (characterStats.name == "Gaku") return 3;
        if (characterStats.name == "Yuniti") return 4;
        if (characterStats.name == "Hanami") return 5;
        if (characterStats.name == "Flynn") return 6;
        if (characterStats.name == "Kuroka") return 7;
        
        Debug.LogWarning($"Nombre de personaje {characterStats.name} no asignado a ningún índice");
        return -1;
    }
}
