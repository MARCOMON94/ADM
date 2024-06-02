using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CombatManager : MonoBehaviour
{
    public static CombatManager Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Attack(TacticsMove attacker, TacticsMove defender)
{
    // Girar al atacante hacia el defensor
    Vector3 directionToTarget = defender.transform.position - attacker.transform.position;
    directionToTarget.y = 0; // Mantener la rotación en el plano horizontal
    attacker.transform.rotation = Quaternion.LookRotation(directionToTarget);

    int damage = attacker.characterStats.basicDamage;
    defender.characterStats.health -= damage;
    defender.UpdateHealthUI();

    if (defender.characterStats.health <= 0)
    {
        TurnManager.RemoveUnit(defender);
        Destroy(defender.gameObject);
    }

    // Ajustar la rotación del atacante para que mire en una dirección recta (norte, sur, este, oeste)
    AdjustRotation(attacker);
}

private void AdjustRotation(TacticsMove unit)
{
    Vector3 direction = unit.transform.forward;
    if (Mathf.Abs(direction.x) > Mathf.Abs(direction.z))
    {
        direction.z = 0;
        direction.x = Mathf.Sign(direction.x);
    }
    else
    {
        direction.x = 0;
        direction.z = Mathf.Sign(direction.z);
    }
    unit.transform.rotation = Quaternion.LookRotation(direction);
}

    public void AttackWithPierce(TacticsMove attacker, TacticsMove defender)
{
    Vector3 direction = (defender.transform.position - attacker.transform.position).normalized;
    float distance = Vector3.Distance(attacker.transform.position, defender.transform.position);


    // Girar al atacante hacia el defensor
    direction.y = 0; // Mantener la rotación en el plano horizontal
    attacker.transform.rotation = Quaternion.LookRotation(direction);

    // Asegúrate de que todas las capas relevantes estén incluidas en la máscara del Raycast
    int layerMask = LayerMask.GetMask("Default", "Player", "NPC"); // Ajusta según tus capas

    RaycastHit[] hits = Physics.RaycastAll(attacker.transform.position, direction, distance, layerMask);

    foreach (RaycastHit hit in hits)
    {
        TacticsMove target = hit.collider.GetComponent<TacticsMove>();
        if (target != null)
        {
            int damage = attacker.characterStats.basicDamage;
            target.characterStats.health -= damage;
            target.UpdateHealthUI();

            if (target.characterStats.health <= 0)
            {
                TurnManager.RemoveUnit(target);
                Destroy(target.gameObject);
            }
        }
    }
}


}

