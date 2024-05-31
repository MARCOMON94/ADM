using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerMove : TacticsMove
{
    private enum PlayerAction { None, Move, Attack }
    private PlayerAction currentAction = PlayerAction.None;
    private bool hasMoved = false;

    void Start()
    {
        Init(true);
        UpdateHealthUI();
    }

    void Update()
    {
        if (!turn)
        {
            return;
        }

        if (!moving)
        {
            if (currentAction == PlayerAction.None)
            {
                // Espera a que se seleccione una acción mediante botones
            }
            else if (currentAction == PlayerAction.Move)
            {
                FindSelectableTiles();
                CheckMouseMovement();
            }
            else if (currentAction == PlayerAction.Attack)
            {
                FindAttackableTiles();
                ShowAttackableEnemies();
                CheckMouseAttack();
            }
        }
        else
        {
            Move();
        }
    }

    public void SetActionMove()
    {
        currentAction = PlayerAction.Move;
    }

    public void SetActionAttack()
{
    currentAction = PlayerAction.Attack;
    // Llama a FindSelectableTiles con ignoreOccupied = true
    FindSelectableTiles(true);
}

    void CheckMouseMovement()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.tag == "Tile")
                {
                    Tile t = hit.collider.GetComponent<Tile>();

                    if (t.selectable)
                    {
                        MoveToTile(t);
                        currentAction = PlayerAction.None;
                        hasMoved = true;
                    }
                }
            }
        }
    }

    void ShowAttackableEnemies()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("NPC");
        foreach (GameObject enemy in enemies)
        {
            TacticsMove enemyMove = enemy.GetComponent<TacticsMove>();
            Tile enemyTile = GetTargetTile(enemy);

            if (enemyTile != null && selectableTiles.Contains(enemyTile))
            {
                Renderer renderer = enemy.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.material.color = Color.red;
                }

                enemyTile.selectable = true;
                enemyTile.GetComponent<Renderer>().material.color = Color.red;
            }
        }
    }

    public void CheckMouseAttack()
{
    if (Input.GetMouseButtonDown(0))
    {
        Debug.Log("Mouse click detectado para ataque");
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            Debug.Log($"Objeto impactado: {hit.collider.name}");

            if (hit.collider.tag == "NPC")
            {
                TacticsMove enemy = hit.collider.GetComponent<TacticsMove>();

                if (enemy != null)
                {
                    float distanceToEnemy = Vector3.Distance(new Vector3(transform.position.x, 0, transform.position.z),
                                                            new Vector3(enemy.transform.position.x, 0, enemy.transform.position.z));
                    Debug.Log($"Distancia al enemigo {enemy.name}: {distanceToEnemy}, rango de ataque: {characterStats.attackRange}");

                    if (distanceToEnemy <= characterStats.attackRange + 0.05f) // Añadimos un pequeño margen para problemas de precisión
                    {
                        if (characterStats.attackType == AttackType.Normal)
                        {
                            Debug.Log($"Realizando ataque normal a {enemy.name}");
                            CombatManager.Instance.Attack(this, enemy);
                        }
                        else if (characterStats.attackType == AttackType.Pierce)
                        {
                            Debug.Log($"Realizando ataque de penetración a {enemy.name}");
                            CombatManager.Instance.AttackWithPierce(this, enemy);
                        }
                        EndTurn();
                    }
                    else
                    {
                        Debug.Log($"Enemigo {enemy.name} fuera de rango");
                    }
                }
                else
                {
                    Debug.Log("Componente TacticsMove no encontrado en el objetivo impactado");
                }
            }
            else
            {
                Debug.Log("Impacto en objeto que no es NPC");
            }
        }
        else
        {
            Debug.Log("Raycast no impactó en ningún objeto");
        }
    }
}


    public void EndTurn()
{
    currentAction = PlayerAction.None;
    hasMoved = false;

    GameObject[] enemies = GameObject.FindGameObjectsWithTag("NPC");
    foreach (GameObject enemy in enemies)
    {
        Renderer renderer = enemy.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = Color.white;
        }
    }

    foreach (Tile tile in selectableTiles)
    {
        tile.selectable = false;
        tile.GetComponent<Renderer>().material.color = Color.white;
    }
    selectableTiles.Clear();

    TurnManager.EndTurn();
}

    public void BeginTurn()
    {
        turn = true;
        currentAction = PlayerAction.None;
        hasMoved = false;
    }

    public override void UpdateHealthUI()
{
    int characterIndex = GetCharacterIndex();
    if (characterIndex != -1)
    {
        Debug.Log($"Actualizando salud de {characterStats.name} en el índice {characterIndex} con salud {characterStats.health}");
        UIManager.Instance.UpdateCharacterHealth(characterIndex, characterStats.health);
    }
}


    private int GetCharacterIndex()
    {
        if (characterStats.name == "Paco") return 0;
        if (characterStats.name == "Loli") return 1;
        if (characterStats.name == "Avelino") return 2;
        if (characterStats.name == "Pepa") return 3;
        
        Debug.LogWarning($"Nombre de personaje {characterStats.name} no asignado a ningún índice");
        return -1;
    }
}