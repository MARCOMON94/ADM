using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;

public class TurnManager : MonoBehaviour
{
    static List<TacticsMove> units = new List<TacticsMove>();

    static int currentIndex = 0;
    static bool isPlayerTurn = true;
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
        isPlayerTurn = true;

        if (units.Count > 0)
        {
            StartTurn(); // Inicia el primer turno
        }
    }

    public static void StartTurn()
    {
        isEndingTurn = false;

        if (units[currentIndex] is PlayerMove)
        {
            UIManager.Instance.ShowPlayerControls(); // Muestra los controles del jugador
            UIManager.Instance.SetCurrentPlayerMove(units[currentIndex] as PlayerMove);
        }
        else
        {
            UIManager.Instance.HidePlayerControls(); // Oculta los controles del jugador
        }

        units[currentIndex].BeginTurn();
    }

    public static void EndTurn()
    {
        if (isEndingTurn) return;
        isEndingTurn = true;

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

    public static void AddUnit(TacticsMove unit)
    {
        units.Add(unit);
        units = units.OrderByDescending(u => u.characterStats.speed).ToList();
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
    }
}
