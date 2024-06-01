using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;

public class TurnManager : MonoBehaviour
{
    static List<TacticsMove> playerUnits = new List<TacticsMove>();
    static List<TacticsMove> npcUnits = new List<TacticsMove>();

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
        if (playerUnits.Count == 0 || npcUnits.Count == 0)
        {
            InitTurnQueue();
        }
    }

    static void InitTurnQueue()
    {
        playerUnits = playerUnits.OrderByDescending(unit => unit.characterStats.speed).ToList();
        npcUnits = npcUnits.OrderByDescending(unit => unit.characterStats.speed).ToList();
        
        currentIndex = 0;
        isPlayerTurn = true;

        if (playerUnits.Count > 0)
        {
            StartTurn(); // Inicia el primer turno
        }
    }

    public static void StartTurn()
    {
        isEndingTurn = false;

        if (isPlayerTurn && playerUnits.Count > 0)
        {
            UIManager.Instance.ShowPlayerControls(); // Muestra los controles del jugador
            playerUnits[currentIndex].BeginTurn();
            UIManager.Instance.SetCurrentPlayerMove(playerUnits[currentIndex] as PlayerMove);
        }
        else if (!isPlayerTurn && npcUnits.Count > 0)
        {
            UIManager.Instance.HidePlayerControls(); // Oculta los controles del jugador
            npcUnits[currentIndex].BeginTurn();
        }
    }

    public static void EndTurn()
    {
        if (isEndingTurn) return;
        isEndingTurn = true;

        if (isPlayerTurn)
        {
            playerUnits[currentIndex].EndTurn();
            currentIndex++;

            if (currentIndex >= playerUnits.Count)
            {
                currentIndex = 0;
                isPlayerTurn = false;
            }
        }
        else
        {
            npcUnits[currentIndex].EndTurn();
            currentIndex++;

            if (currentIndex >= npcUnits.Count)
            {
                currentIndex = 0;
                isPlayerTurn = true;
                currentRound++;
                Debug.Log("Ronda " + currentRound);
            }
        }

        isEndingTurn = false;
        StartTurn();
    }

    public static void AddUnit(TacticsMove unit, bool isPlayer)
    {
        if (isPlayer)
        {
            playerUnits.Add(unit);
            playerUnits = playerUnits.OrderByDescending(u => u.characterStats.speed).ToList();
        }
        else
        {
            npcUnits.Add(unit);
            npcUnits = npcUnits.OrderByDescending(u => u.characterStats.speed).ToList();
        }
    }

    public static void RemoveUnit(TacticsMove unit, bool isPlayer)
    {
        if (isPlayer)
        {
            int index = playerUnits.IndexOf(unit);
            if (index != -1)
            {
                playerUnits.RemoveAt(index);
                if (index <= currentIndex && currentIndex > 0)
                {
                    currentIndex--;
                }
            }
            if (UIManager.Instance.GetCurrentPlayerMove() == unit)
            {
                UIManager.Instance.SetCurrentPlayerMove(null);
            }
        }
        else
        {
            int index = npcUnits.IndexOf(unit);
            if (index != -1)
            {
                npcUnits.RemoveAt(index);
                if (index <= currentIndex && currentIndex > 0)
                {
                    currentIndex--;
                }
            }
        }
    }
}



