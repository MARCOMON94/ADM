using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;

public class TurnManagerPvP : MonoBehaviour
{
    static List<TacticsMove> units = new List<TacticsMove>();
    static int currentIndex = 0;
    static int currentRound = 1;
    static bool isEndingTurn = false;

    public UIManagerPvP uiManager;

    void Start()
    {
        // Inicializa el TurnManagerPvP, registrando los indicadores de turno de los personajes y configurando la cola de turnos.
        string[] characterNames = new string[] { "Nekomaru", "Aya", "Gaku", "Hanami", "Flynn", "Kuroka" };

        foreach (string name in characterNames)
        {
            GameObject character = GameObject.Find(name);
            if (character != null)
            {
                TacticsMove unit = character.GetComponent<TacticsMove>();
                if (unit != null)
                {
                    AddUnit(unit);
                    UIManagerPvP.Instance.RegisterCharacterTurnIndicator(name, units.Count - 1);
                }
            }
        }

        InitTurnQueue();
        UIManagerPvP.Instance.UpdateRoundText(currentRound);
    }

    void Update()
    {
        // Si no hay unidades en la cola, la inicializa.
        if (units.Count == 0)
        {
            InitTurnQueue();
        }
    }

    static void InitTurnQueue()
    {
        // Ordena las unidades por velocidad y comienza el turno de la primera unidad en la cola.
        units = units.OrderByDescending(unit => unit.characterStats.speed).ToList();
        currentIndex = 0;

        if (units.Count > 0)
        {
            StartTurn();
        }
    }

    public static void StartTurn()
    {
        // Inicia el turno de la unidad actual, mostrando u ocultando los controles del jugador según corresponda.
        isEndingTurn = false;

        TacticsMove currentUnit = units[currentIndex];

        if (currentUnit is PlayerMovePvP)
        {
            UIManagerPvP.Instance.ShowPlayerControls();
            UIManagerPvP.Instance.SetCurrentPlayerMove(currentUnit as PlayerMovePvP);
        }
        else
        {
            UIManagerPvP.Instance.HidePlayerControls();
        }

        currentUnit.BeginTurn();
        UIManagerPvP.Instance.UpdateTurnFrame(currentUnit.characterStats.name);
    }

    public static void EndTurn()
    {
        // Finaliza el turno de la unidad actual, actualiza el índice de la unidad actual y verifica el estado del juego.
        if (isEndingTurn) return;
        isEndingTurn = true;

        if (units.Count == 0)
        {
            isEndingTurn = false;
            return;
        }

        units[currentIndex].EndTurn();
        currentIndex++;

        if (currentIndex >= units.Count)
        {
            currentIndex = 0;
            currentRound++;
            UIManagerPvP.Instance.UpdateRoundText(currentRound);
        }

        CheckGameOver();
        isEndingTurn = false;

        if (units.Count > 0)
        {
            StartTurn();
        }
    }

    public static void AddUnit(TacticsMove unit)
    {
        // Añade una unidad a la lista y la ordena por velocidad.
        units.Add(unit);
        units = units.OrderByDescending(u => u.characterStats.speed).ToList();
    }

    public static void RemoveUnit(TacticsMove unit)
    {
        // Elimina una unidad de la lista y ajusta el índice de la unidad actual según corresponda.
        int index = units.IndexOf(unit);
        if (index != -1)
        {
            units.RemoveAt(index);

            if (index < currentIndex)
            {
                currentIndex--;
            }
            else if (index == currentIndex)
            {
                currentIndex = currentIndex % units.Count;
            }
            
            UIManagerPvP.Instance.DeactivateTurnFrame(unit.characterStats.name);
        }
    }

    public static List<TacticsMove> GetUnits()
    {
        // Devuelve la lista de unidades.
        return units;
    }

    private static void CheckGameOver()
    {
        // Verifica si todos los jugadores están muertos y muestra la pantalla de fin de juego si es así.
        bool allPlayersDead = !units.Any(unit => unit is PlayerMovePvP);
        if (allPlayersDead)
        {
            UIManagerPvP.Instance.ShowGameOverScreen("Game Over");
        }
    }

    public static void ResetTurnManager()
    {
        // Reinicia el TurnManagerPvP a su estado inicial.
        units.Clear();
        currentIndex = 0;
        currentRound = 1;
        isEndingTurn = false;
    }
}
