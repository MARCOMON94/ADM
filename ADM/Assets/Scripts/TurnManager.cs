using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;

public class TurnManager : MonoBehaviour
{
    static List<TacticsMove> units = new List<TacticsMove>();

    static int currentIndex = 0;
    static int currentRound = 1;
    static bool isEndingTurn = false;

    public UIManager uiManager; // Referencia al UIManager

    void Start()
    {
        InitTurnQueue(); // Inicializa la cola de turnos al inicio
    }

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
            StartTurn(); // Inicia el primer turno
        }
    }

    public static void StartTurn()
    {
        Debug.Log("StartTurn called");
        isEndingTurn = false;

        if (units.Count > 0)
        {
            TacticsMove currentUnit = units[currentIndex];
            Debug.Log($"{currentUnit.characterStats.name}'s turn with speed {currentUnit.characterStats.speed}");

            if (currentUnit is PlayerMove)
            {
                Debug.Log("Player's turn");
                UIManager.Instance.ShowPlayerControls(); // Muestra los controles del jugador
                UIManager.Instance.SetCurrentPlayerMove(currentUnit as PlayerMove);
            }
            else
            {
                Debug.Log("NPC's turn");
                UIManager.Instance.HidePlayerControls(); // Oculta los controles del jugador
            }

            currentUnit.BeginTurn();
        }
    }

    public static void EndTurn()
    {
        Debug.Log("EndTurn called");
        if (isEndingTurn) return;
        isEndingTurn = true;

        if (units.Count > 0)
        {
            units[currentIndex].EndTurn();
            currentIndex++;

            if (currentIndex >= units.Count)
            {
                currentIndex = 0;
                currentRound++;
                Debug.Log("Ronda " + currentRound);
            }

            isEndingTurn = false;
            StartTurn();
        }
    }

    public static void AddUnit(TacticsMove unit)
    {
        units.Add(unit);
        units = units.OrderByDescending(u => u.characterStats.speed).ToList();
        InitTurnQueue(); // Reinitialize turn queue after adding unit
    }

    public static void RemoveUnit(TacticsMove unit)
    {
        int index = units.IndexOf(unit);
        if (index != -1)
        {
            units.RemoveAt(index);
            if (index <= currentIndex && currentIndex > 0)
            {
                currentIndex--;
            }
        }
        InitTurnQueue(); // Reinitialize turn queue after removing unit
    }
}
