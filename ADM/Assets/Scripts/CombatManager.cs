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
        int damage = attacker.characterStats.basicDamage;
        defender.characterStats.health -= damage;
        Debug.Log($"Salud de {defender.name} después del ataque: {defender.characterStats.health}");
        defender.UpdateHealthUI();

        Debug.Log(attacker.name + " ataca a " + defender.name + " causando " + damage + " daño.");

        if (defender.characterStats.health <= 0)
        {
            Debug.Log(defender.name + " ha muerto.");
            TurnManager.RemoveUnit(defender, defender is PlayerMove);
            Destroy(defender.gameObject);
        }
    }
}
