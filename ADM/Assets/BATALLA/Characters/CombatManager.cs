using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatManager : MonoBehaviour
{
    public static void HandleCombat(TacticsMove attacker, TacticsMove defender)
    {
        attacker.Attack(defender);
        if (defender.characterStats.health > 0)
        {
            defender.Attack(attacker);
        }
    }
}
