using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatManager : MonoBehaviour
{
    // Instancia única del CombatManager
    public static CombatManager Instance;

    void Awake()
    {
        // Implementación del patrón Singleton para asegurar que solo haya una instancia del CombatManager
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject); // Destruye cualquier instancia adicional
        }
    }

    // Método para realizar un ataque entre un atacante y un defensor
    public void Attack(TacticsMove attacker, TacticsMove defender)
    {
        int damage = attacker.characterStats.basicDamage; // Daño básico del atacante
        defender.characterStats.health -= damage; // Reduce la vida del defensor por el daño recibido

        Debug.Log(attacker.name + " ataca a " + defender.name + " causando " + damage + " daño.");

        // Si la vida del defensor llega a 0 o menos, se maneja su "muerte"
        if (defender.characterStats.health <= 0)
        {
            Debug.Log(defender.name + " ha muerto.");
            // Elimina la unidad del TurnManager y destruye su GameObject
            TurnManager.RemoveUnit(defender, defender is PlayerMove);
            Destroy(defender.gameObject);
        }
    }
}
