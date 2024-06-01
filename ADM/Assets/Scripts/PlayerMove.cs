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

        float heightDifference = Mathf.Abs(enemy.transform.position.y - transform.position.y);

        // Añadimos la verificación de heightAttack aquí
        if (enemyTile != null && selectableTiles.Contains(enemyTile) && 
            (characterStats.heightAttack || heightDifference <= 0.1f)) // Solo mostrar si la diferencia de altura es mínima
        {
            Renderer renderer = enemy.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = Color.red;
            }
        }
    }
}
    public void CheckMouseAttack()
{
    if (Input.GetMouseButtonDown(0))
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.tag == "NPC")
            {
                TacticsMove enemy = hit.collider.GetComponent<TacticsMove>();

                if (enemy != null)
                {
                    float distanceToEnemy = Vector3.Distance(new Vector3(transform.position.x, 0, transform.position.z),
                                                            new Vector3(enemy.transform.position.x, 0, enemy.transform.position.z));
                    float heightDifference = Mathf.Abs(enemy.transform.position.y - transform.position.y);

                    // Añadimos la verificación de heightAttack aquí
                    if (distanceToEnemy <= characterStats.attackRange + 0.05f && 
                        (characterStats.heightAttack || heightDifference <= 0.1f)) // Solo permitir ataques si la diferencia de altura es mínima
                    {
                        if (characterStats.attackType == AttackType.Normal)
                        {
                            CombatManager.Instance.Attack(this, enemy);
                        }
                        else if (characterStats.attackType == AttackType.Pierce)
                        {
                            CombatManager.Instance.AttackWithPierce(this, enemy);
                        }
                        EndTurn();
                    }
                }
            }
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
        if (characterStats.name == "Nekomaru") return 0;
        if (characterStats.name == "Aya") return 1;
        if (characterStats.name == "Umi") return 2;
        if (characterStats.name == "Gaku") return 3;
        if (characterStats.name == "Yuniti") return 4;
        if (characterStats.name == "Hanami") return 5;
        if (characterStats.name == "Flynn") return 6;
        if (characterStats.name == "Kuroka") return 7;
        
        Debug.LogWarning($"Nombre de personaje {characterStats.name} no asignado a ningún índice");
        return -1;
    }
}