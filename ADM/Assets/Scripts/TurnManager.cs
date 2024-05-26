using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TurnManager : MonoBehaviour
{
    static List<TacticsMove> playerUnits = new List<TacticsMove>(); // Lista de unidades de jugador
    static List<TacticsMove> npcUnits = new List<TacticsMove>(); // Lista de unidades de NPC
    static int currentIndex = 0; // Índice actual de la unidad en turno
    static bool isPlayerTurn = true; // Indica si es el turno del jugador
    static int currentRound = 1; // Número de la ronda actual

    // Método Start que se llama al inicio
    void Start()
    {
        InitTurnQueue(); // Inicializa la cola de turnos
    }

    // Método Update que se llama una vez por frame
    void Update()
    {
        if (playerUnits.Count == 0 || npcUnits.Count == 0)
        {
            InitTurnQueue(); // Si no hay unidades, inicializa la cola de turnos
        }
    }

    // Método estático para inicializar la cola de turnos
    static void InitTurnQueue()
    {
        playerUnits = playerUnits.OrderByDescending(unit => unit.characterStats.speed).ToList(); // Ordena las unidades por velocidad
        npcUnits = npcUnits.OrderByDescending(unit => unit.characterStats.speed).ToList();
        currentIndex = 0; // Resetea el índice actual
        isPlayerTurn = true; // Establece el turno del jugador

        if (playerUnits.Count > 0)
        {
            StartTurn(); // Inicia el turno de la primera unidad
        }
    }

    // Método estático para iniciar el turno de la unidad actual
    public static void StartTurn()
    {
        if (isPlayerTurn && playerUnits.Count > 0)
        {
            playerUnits[currentIndex].BeginTurn(); // Llama al método BeginTurn de la unidad actual
        }
        else if (!isPlayerTurn && npcUnits.Count > 0)
        {
            npcUnits[currentIndex].BeginTurn(); // Llama al método BeginTurn de la unidad actual
        }
    }

    // Método estático para finalizar el turno de la unidad actual
    public static void EndTurn()
    {
        if (isPlayerTurn)
        {
            playerUnits[currentIndex].EndTurn(); // Llama al método EndTurn de la unidad actual
            currentIndex++;

            if (currentIndex >= playerUnits.Count)
            {
                currentIndex = 0;
                isPlayerTurn = false; // Cambia al turno de los NPCs
            }
        }
        else
        {
            npcUnits[currentIndex].EndTurn(); // Llama al método EndTurn de la unidad actual
            currentIndex++;

            if (currentIndex >= npcUnits.Count)
            {
                currentIndex = 0;
                isPlayerTurn = true; // Cambia al turno de los jugadores
                currentRound++;
                Debug.Log("Ronda " + currentRound); // Incrementa el número de rondas y lo muestra en el log
            }
        }

        StartTurn(); // Inicia el turno de la siguiente unidad
    }

    // Método estático para añadir una unidad a la cola de turnos
    public static void AddUnit(TacticsMove unit, bool isPlayer)
    {
        if (isPlayer)
        {
            playerUnits.Add(unit); // Añade la unidad a la lista de jugadores
            playerUnits = playerUnits.OrderByDescending(u => u.characterStats.speed).ToList(); // Reordena las unidades por velocidad
        }
        else
        {
            npcUnits.Add(unit); // Añade la unidad a la lista de NPCs
            npcUnits = npcUnits.OrderByDescending(u => u.characterStats.speed).ToList(); // Reordena las unidades por velocidad
        }
    }

    // Método opcional para eliminar una unidad de la cola de turnos si es necesario
    public static void RemoveUnit(TacticsMove unit, bool isPlayer)
    {
        if (isPlayer)
        {
            playerUnits.Remove(unit); // Elimina la unidad de la lista de jugadores
        }
        else
        {
            npcUnits.Remove(unit); // Elimina la unidad de la lista de NPCs
        }
    }
}
