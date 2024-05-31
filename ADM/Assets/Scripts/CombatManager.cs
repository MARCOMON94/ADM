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
        Debug.Log($"{attacker.name} ataca a {defender.name}");
        int damage = attacker.characterStats.basicDamage;
        defender.characterStats.health -= damage;
        Debug.Log($"Salud de {defender.name} después del ataque: {defender.characterStats.health}");
        defender.UpdateHealthUI();

        Debug.Log(attacker.name + " causó " + damage + " daño a " + defender.name);

        if (defender.characterStats.health <= 0)
        {
            Debug.Log(defender.name + " ha muerto.");
            TurnManager.RemoveUnit(defender, defender is PlayerMove);
            Destroy(defender.gameObject);
        }
    }

    public void AttackWithPierce(TacticsMove attacker, TacticsMove defender)
{
    Vector3 direction = (defender.transform.position - attacker.transform.position).normalized;
    float distance = Vector3.Distance(attacker.transform.position, defender.transform.position);

    Debug.Log($"Ataque con penetración de {attacker.name} a {defender.name} en dirección {direction} y distancia {distance}");

    // Asegúrate de que todas las capas relevantes estén incluidas en la máscara del Raycast
    int layerMask = LayerMask.GetMask("Default", "Player", "NPC"); // Ajusta según tus capas

    RaycastHit[] hits = Physics.RaycastAll(attacker.transform.position, direction, distance, layerMask);

    Debug.Log($"Número de impactos detectados: {hits.Length}");
    foreach (RaycastHit hit in hits)
    {
        Debug.Log($"Impacto detectado en objeto: {hit.collider.name} con tag: {hit.collider.tag}");
        TacticsMove target = hit.collider.GetComponent<TacticsMove>();
        if (target != null)
        {
            Debug.Log($"{attacker.name} golpea a {target.name} con ataque de penetración");
            int damage = attacker.characterStats.basicDamage;
            target.characterStats.health -= damage;
            Debug.Log($"Salud de {target.name} después del ataque: {target.characterStats.health}");
            target.UpdateHealthUI();

            if (target.characterStats.health <= 0)
            {
                Debug.Log(target.name + " ha muerto.");
                TurnManager.RemoveUnit(target, target is PlayerMove);
                Destroy(target.gameObject);
            }
        }
    }
}


}

