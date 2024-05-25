using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TurnManager : MonoBehaviour
{
    static List<TacticsMove> units = new List<TacticsMove>();
    static int currentIndex = 0;
    static int currentRound = 1;

    // Método Start que se llama al inicio
    void Start()
    {
        InitTurnQueue(); // Inicializa la cola de turnos
    }

    // Método Update que se llama una vez por frame
    void Update()
    {
        if (units.Count == 0)
        {
            InitTurnQueue(); // Si no hay unidades, inicializa la cola de turnos
        }
    }

    // Método estático para inicializar la cola de turnos
    static void InitTurnQueue()
    {
        units = units.OrderByDescending(unit => unit.characterStats.speed).ToList(); // Ordena las unidades por velocidad
        currentIndex = 0;

        if (units.Count > 0)
        {
            StartTurn(); // Inicia el turno de la primera unidad
        }
    }

    // Método estático para iniciar el turno de la unidad actual
    public static void StartTurn()
    {
        if (units.Count > 0)
        {
            units[currentIndex].BeginTurn(); // Llama al método BeginTurn de la unidad actual
        }
    }

    // Método estático para finalizar el turno de la unidad actual
    public static void EndTurn()
    {
        units[currentIndex].EndTurn(); // Llama al método EndTurn de la unidad actual
        currentIndex++;

        if (currentIndex >= units.Count)
        {
            currentIndex = 0;
            currentRound++;
            Debug.Log("Ronda " + currentRound); // Incrementa el número de rondas y lo muestra en el log
        }

        StartTurn(); // Inicia el turno de la siguiente unidad
    }

    // Método estático para añadir una unidad a la cola de turnos
    public static void AddUnit(TacticsMove unit)
    {
        units.Add(unit);
        units = units.OrderByDescending(u => u.characterStats.speed).ToList(); // Reordena las unidades por velocidad
    }

    // Método opcional para eliminar una unidad de la cola de turnos si es necesario
    public static void RemoveUnit(TacticsMove unit)
    {
        units.Remove(unit);
    }
}
