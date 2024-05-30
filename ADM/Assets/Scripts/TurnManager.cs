using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;

public class TurnManager : MonoBehaviour
{
    // Listas de unidades de jugadores y NPCs
    static List<TacticsMove> playerUnits = new List<TacticsMove>();
    static List<TacticsMove> npcUnits = new List<TacticsMove>();
    
    // Índice del turno actual y bandera para determinar si es turno del jugador
    static int currentIndex = 0;
    static bool isPlayerTurn = true;
    static int currentRound = 1;
    static bool isEndingTurn = false; // Añadimos esta bandera

    public UIManager uiManager; // Referencia al UIManager

    void Start()
    {
        InitTurnQueue(); // Inicializa la cola de turnos al inicio
    }

    void Update()
    {
        // Si no hay unidades de jugadores o NPCs, inicializa la cola de turnos
        if (playerUnits.Count == 0 || npcUnits.Count == 0)
        {
            InitTurnQueue();
        }
    }

    static void InitTurnQueue()
    {
        // Ordena las unidades por velocidad de forma descendente
        playerUnits = playerUnits.OrderByDescending(unit => unit.characterStats.speed).ToList();
        npcUnits = npcUnits.OrderByDescending(unit => unit.characterStats.speed).ToList();
        
        currentIndex = 0; // Resetea el índice de turno
        isPlayerTurn = true; // Empieza con el turno del jugador

        if (playerUnits.Count > 0)
        {
            StartTurn(); // Inicia el primer turno
        }
    }

    public static void StartTurn()
    {
        isEndingTurn = false; // Reseteamos la bandera al iniciar un nuevo turno
        if (isPlayerTurn && playerUnits.Count > 0)
        {
            playerUnits[currentIndex].BeginTurn();
            UIManager.Instance.SetCurrentPlayerMove(playerUnits[currentIndex] as PlayerMove);
            UIManager.Instance.TogglePlayerControls(true); // Habilitar controles de jugador
        }
        else if (!isPlayerTurn && npcUnits.Count > 0)
        {
            npcUnits[currentIndex].BeginTurn();
        }
    }

    public static void EndTurn()
    {
        if (isEndingTurn) return; // Si ya estamos en el proceso de finalizar un turno, salimos
        isEndingTurn = true; // Marcamos que estamos finalizando un turno

        // Deshabilitar controles de jugador al finalizar el turno
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

        isEndingTurn = false; // Reseteamos la bandera al finalizar el turno
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
            playerUnits.Remove(unit);
        }
        else
        {
            npcUnits.Remove(unit);
        }
    }
}
