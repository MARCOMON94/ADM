using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TurnManager : MonoBehaviour
{
    static List<TacticsMove> units = new List<TacticsMove>();
    static int currentIndex = 0;
    static int currentRound = 1;

    // Start is called before the first frame update
    void Start()
    {
        InitTurnQueue();
    }

    // Update is called once per frame
    void Update()
    {
        if (units.Count == 0)
        {
            InitTurnQueue();
        }
    }

    static void InitTurnQueue()
    {
        units = units.OrderByDescending(unit => unit.characterStats.speed).ToList();
        currentIndex = 0;

        if (units.Count > 0)
        {
            StartTurn();
        }
    }

    public static void StartTurn()
    {
        if (units.Count > 0)
        {
            units[currentIndex].BeginTurn();
        }
    }

    public static void EndTurn()
    {
        units[currentIndex].EndTurn();
        currentIndex++;

        if (currentIndex >= units.Count)
        {
            currentIndex = 0;
            currentRound++;
            Debug.Log("Ronda " + currentRound);
        }

        StartTurn();
    }

    public static void AddUnit(TacticsMove unit)
    {
        units.Add(unit);
        units = units.OrderByDescending(u => u.characterStats.speed).ToList();
    }

    // Opcional: función para eliminar un personaje si es necesario
    public static void RemoveUnit(TacticsMove unit)
    {
        units.Remove(unit);
    }
}