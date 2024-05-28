using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : TacticsMove
{
    // Enum para las posibles acciones del jugador
    private enum PlayerAction { None, Move, Attack }

    // Acción actual del jugador
    private PlayerAction currentAction = PlayerAction.None;

    // Indica si el jugador ya se ha movido en este turno
    private bool hasMoved = false;

    void Start()
    {
        // Inicializa el jugador usando el método Init de TacticsMove
        Init(true);
    }

    void Update()
    {
        // Si no es el turno del jugador, no hace nada
        if (!turn)
        {
            return;
        }

        // Si el jugador no está en movimiento
        if (!moving)
        {
            if (currentAction == PlayerAction.None)
            {
                // Espera a que se seleccione una acción mediante botones
            }
            else if (currentAction == PlayerAction.Move)
            {
                // Encuentra los tiles seleccionables y revisa el movimiento del ratón para moverse
                FindSelectableTiles();
                CheckMouseMovement();
            }
            else if (currentAction == PlayerAction.Attack)
            {
                // Encuentra los tiles en el rango de ataque, muestra los enemigos que se pueden atacar y revisa el movimiento del ratón para atacar
                FindAttackableTiles();
                ShowAttackableEnemies();
                CheckMouseAttack();
            }
        }
        else
        {
            // Mueve al jugador
            Move();
        }
    }

    // Métodos para establecer la acción actual desde los botones
    public void SetActionMove()
    {
        currentAction = PlayerAction.Move;
    }

    public void SetActionAttack()
    {
        currentAction = PlayerAction.Attack;
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
                        // Mueve al jugador al tile
                        MoveToTile(t);
                        // Resetea la acción actual y marca que el jugador ya se ha movido
                        currentAction = PlayerAction.None;
                        hasMoved = true;
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
                        // Realiza un ataque
                        CombatManager.Instance.Attack(this, enemy);
                        // Termina el turno del jugador
                        EndTurn();
                    }
                }
            }
        }
    }

    // Termina el turno del jugador
    public void EndTurn()
    {
        // Resetea la acción actual y el estado de movimiento
        currentAction = PlayerAction.None;
        hasMoved = false;

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
        selectableTiles.Clear();

        // Notifica al TurnManager para finalizar el turno
        TurnManager.EndTurn();
    }

    public void BeginTurn()
    {
        // Marca el turno como activo, resetea la acción actual y el estado de movimiento
        turn = true;
        currentAction = PlayerAction.None;
        hasMoved = false;
        // No mostrar el menú de acciones porque ahora se maneja con botones
    }
}
