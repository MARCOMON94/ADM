using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : TacticsMove
{
    private enum PlayerAction { None, Move, Attack } // Enum para las posibles acciones del jugador
    private PlayerAction currentAction = PlayerAction.None; // Acción actual del jugador
    private bool hasMoved = false; // Indica si el jugador ya se ha movido en este turno

    void Start()
    {
        Init(true); // Inicializa el jugador usando el método Init de TacticsMove
    }

    void Update()
    {
        if (!turn) // Si no es el turno del jugador, no hace nada
        {
            return;
        }

        if (!moving) // Si el jugador no está en movimiento
        {
            if (currentAction == PlayerAction.None) // Si no hay una acción actual
            {
                ShowActionMenu(); // Muestra el menú de acciones
            }
            else if (currentAction == PlayerAction.Move) // Si la acción actual es mover
            {
                FindSelectableTiles(); // Encuentra los tiles seleccionables
                CheckMouseMovement(); // Revisa el movimiento del ratón para moverse
            }
            else if (currentAction == PlayerAction.Attack) // Si la acción actual es atacar
            {
                FindAttackableTiles(); // Encuentra los tiles en el rango de ataque
                ShowAttackableEnemies(); // Muestra los enemigos que se pueden atacar
                CheckMouseAttack(); // Revisa el movimiento del ratón para atacar
            }
        }
        else
        {
            Move(); // Mueve al jugador
        }
    }

    // Muestra el menú de acciones al jugador (mover, atacar, terminar turno)
    void ShowActionMenu()
    {
        if (Input.GetKeyDown(KeyCode.M)) // Si se presiona la tecla M
        {
            currentAction = PlayerAction.Move; // Establece la acción actual como mover
        }
        else if (Input.GetKeyDown(KeyCode.A)) // Si se presiona la tecla A
        {
            currentAction = PlayerAction.Attack; // Establece la acción actual como atacar
        }
        else if (Input.GetKeyDown(KeyCode.T)) // Si se presiona la tecla T
        {
            EndTurn(); // Termina el turno del jugador
        }
    }

    // Revisa el movimiento del ratón para moverse
    void CheckMouseMovement()
    {
        if (Input.GetMouseButtonDown(0)) // Si se presiona el botón izquierdo del ratón
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit)) // Si el rayo golpea algo
            {
                if (hit.collider.tag == "Tile") // Si el objeto golpeado tiene la etiqueta "Tile"
                {
                    Tile t = hit.collider.GetComponent<Tile>();

                    if (t.selectable) // Si el tile es seleccionable
                    {
                        MoveToTile(t); // Mueve al jugador al tile
                        currentAction = PlayerAction.None; // Resetea la acción actual
                        hasMoved = true; // Marca que el jugador ya se ha movido
                    }
                }
            }
        }
    }

    // Muestra los enemigos que se pueden atacar
    void ShowAttackableEnemies()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("NPC"); // Encuentra todos los NPCs
        foreach (GameObject enemy in enemies)
        {
            TacticsMove enemyMove = enemy.GetComponent<TacticsMove>();
            Tile enemyTile = GetTargetTile(enemy);

            if (enemyTile != null && selectableTiles.Contains(enemyTile))
            {
                Renderer renderer = enemy.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.material.color = Color.red; // Resalta al enemigo
                }

                enemyTile.selectable = true; // Marca el tile del enemigo como seleccionable
                enemyTile.GetComponent<Renderer>().material.color = Color.red; // Resalta el tile del enemigo
            }
        }
    }

    // Revisa el movimiento del ratón para atacar
    void CheckMouseAttack()
    {
        if (Input.GetMouseButtonDown(0)) // Si se presiona el botón izquierdo del ratón
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit)) // Si el rayo golpea algo
            {
                if (hit.collider.tag == "NPC") // Si el objeto golpeado tiene la etiqueta "NPC"
                {
                    TacticsMove enemy = hit.collider.GetComponent<TacticsMove>();

                    if (enemy != null && Vector3.Distance(new Vector3(transform.position.x, 0, transform.position.z), 
                                                      new Vector3(enemy.transform.position.x, 0, enemy.transform.position.z)) <= characterStats.attackRange)
                    {
                        CombatManager.Instance.Attack(this, enemy); // Realiza un ataque
                        EndTurn(); // Termina el turno del jugador
                    }
                }
            }
        }
    }

    // Termina el turno del jugador
    public void EndTurn()
    {
        currentAction = PlayerAction.None; // Resetea la acción actual
        hasMoved = false; // Resetea el estado de movimiento

        // Resetea los colores de los enemigos
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("NPC");
        foreach (GameObject enemy in enemies)
        {
            Renderer renderer = enemy.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = Color.white;
            }
        }

        // Resetea los tiles seleccionables
        foreach (Tile tile in selectableTiles)
        {
            tile.selectable = false;
            tile.GetComponent<Renderer>().material.color = Color.white;
        }

        TurnManager.EndTurn(); // Notifica al TurnManager para finalizar el turno
    }

    public void BeginTurn()
    {
        turn = true; // Marca el turno como activo
        currentAction = PlayerAction.None; // Resetea la acción actual
        hasMoved = false; // Resetea el estado de movimiento
        ShowActionMenu(); // Muestra el menú de acciones
    }
}
