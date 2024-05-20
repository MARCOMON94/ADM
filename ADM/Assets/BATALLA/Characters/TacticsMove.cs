using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TacticsMove : MonoBehaviour
{
    public bool turn = false; // Indica si es el turno del personaje
    List<Tile> selectableTiles = new List<Tile>(); // Lista de tiles seleccionables
    GameObject[] tiles; // Array de todos los tiles en el mapa

    Stack<Tile> path = new Stack<Tile>(); // Pila que representa el camino a seguir
    Tile currentTile; // Tile actual en el que se encuentra el personaje

    public bool moving = false; // Indica si el personaje está en movimiento
    public CharacterStatsSO characterStats; // Usar ScriptableObject para estadísticas

    Vector3 velocity = new Vector3(); // Velocidad actual del personaje
    Vector3 heading = new Vector3(); // Dirección del movimiento

    float halfHeight = 0; // Mitad de la altura del colisionador del personaje

    bool fallingDown = false; // Indica si el personaje está cayendo
    bool jumpingUp = false; // Indica si el personaje está saltando
    bool movingEdge = false; // Indica si el personaje se está moviendo hacia el borde
    Vector3 jumpTarget; // Objetivo del salto

    public Tile actualTargetTile; // Tile objetivo final



    // Método de inicialización
    protected void Init()
    {
        tiles = GameObject.FindGameObjectsWithTag("Tile"); // Encuentra todos los tiles en el mapa
        halfHeight = GetComponent<Collider>().bounds.extents.y; // Obtiene la mitad de la altura del colisionador
        TurnManager.AddUnit(this); // Añade este personaje al TurnManager
    }


    // Método para iniciar el turno del personaje
    public void BeginTurn()
    {
        turn = true;
        // Logica para manejar acciones del turno
    }

    // Método para finalizar el turno del personaje
    public void EndTurn()
    {
        turn = false;
    }



    // Obtiene el tile actual en el que se encuentra el personaje
    public void GetCurrentTile()
    {
        currentTile = GetTargetTile(gameObject);
        currentTile.current = true;
    }

    // Obtiene el tile debajo de un GameObject específico
    public Tile GetTargetTile(GameObject target)
    {
        RaycastHit hit;
        Tile tile = null;

        if (Physics.Raycast(target.transform.position, -Vector3.up, out hit, 1))
        {
            tile = hit.collider.GetComponent<Tile>();
        }
        return tile;
    }

    // Calcula la lista de tiles adyacentes teniendo en cuenta la altura de salto
    public void ComputeAdjacencyLists(float jumpHeight, Tile target)
    {
        foreach (GameObject tile in tiles)
        {
            Tile t = tile.GetComponent<Tile>();
            t.FindNeighbors(jumpHeight, target);
        }
    }

    // Encuentra los tiles seleccionables dentro del rango de movimiento
   public void FindSelectableTiles()
{
    ComputeAdjacencyLists(characterStats.jumpHeight, null);
    GetCurrentTile();

    Queue<Tile> process = new Queue<Tile>();
    process.Enqueue(currentTile);
    currentTile.visited = true;

    while (process.Count > 0)
    {
        Tile t = process.Dequeue();
        selectableTiles.Add(t);
        t.selectable = true;

        if (t.distance < characterStats.move)
        {
            foreach (Tile tile in t.adjacencyList)
            {
                if (!tile.visited)
                {
                    tile.parent = t;
                    tile.visited = true;

                    // Calcular la diferencia de altura y ajustar el costo
                    float heightDifference = Mathf.Abs(tile.transform.position.y - t.transform.position.y);
                    int additionalCost = (int)(heightDifference * 2); // Cada unidad de altura cuesta 2 unidades de movimiento

                    // Calcular la nueva distancia total
                    int newDistance = t.distance + 1 + additionalCost;


                    if (newDistance <= characterStats.move)
                    {
                        tile.distance = newDistance;
                        process.Enqueue(tile);
                    }
                }
            }
        }
    }
}




    // Mueve al personaje hacia un tile específico
    public void MoveToTile(Tile tile)
    {
        path.Clear();
        tile.target = true;
        moving = true;

        Tile next = tile;
        while (next != null)
        {
            path.Push(next);
            next = next.parent;
        }
    }

    // Método para mover al personaje
    public void Move()
    {
        if (path.Count > 0)
        {
            Tile t = path.Peek();
            Vector3 target = t.transform.position;

            // Calcula la posición del personaje sobre el tile objetivo
            target.y += halfHeight + t.GetComponent<Collider>().bounds.extents.y;

            if (Vector3.Distance(transform.position, target) >= 0.05f)
            {
                bool jump = transform.position.y != target.y;

                if (jump)
                {
                    Jump(target);
                }
                else
                {
                    CalculateHeading(target);
                    SetHorizontalVelocity();
                }

                // Mueve al personaje
                transform.forward = heading;
                transform.position += velocity * Time.deltaTime;
            }
            else
            {
                // El personaje ha alcanzado el centro del tile
                transform.position = target;
                path.Pop();
                ResetMovementStates();
            }
        }
        else
        {
            RemoveSelectableTiles();
            moving = false;
            ResetMovementStates();
            TurnManager.EndTurn();
        }
    }

    // Resetea los estados de movimiento del personaje
    void ResetMovementStates()
    {
        fallingDown = false;
        jumpingUp = false;
        movingEdge = false;
    }

    // Elimina los tiles seleccionables
    protected void RemoveSelectableTiles()
    {
        if (currentTile != null)
        {
            currentTile.current = false;
            currentTile = null;
        }

        foreach (Tile tile in selectableTiles)
        {
            tile.Reset();
        }

        selectableTiles.Clear();
    }

    // Calcula la dirección hacia el objetivo
    void CalculateHeading(Vector3 target)
    {
        heading = target - transform.position;
        heading.Normalize();
    }

    // Establece la velocidad horizontal
    void SetHorizontalVelocity()
    {
        velocity = heading * characterStats.moveSpeed;
    }

    // Método para manejar el salto
    void Jump(Vector3 target)
    {
        if (fallingDown)
        {
            FallDownward(target);
        }
        else if (jumpingUp)
        {
            JumpUpward(target);
        }
        else if (movingEdge)
        {
            MoveToEdge();
        }
        else
        {
            PrepareJump(target);
        }
    }

    // Prepara el salto
    void PrepareJump(Vector3 target)
    {
        float targetY = target.y; // Guarda el valor original de target.y
        Vector3 localTarget = target; // Crea una copia local de target para modificar
        localTarget.y = transform.position.y;

        CalculateHeading(localTarget); // Usa la copia local para los cálculos

        if (transform.position.y > targetY)
        {
            fallingDown = false;
            jumpingUp = false;
            movingEdge = true;
            jumpTarget = transform.position + (localTarget - transform.position) / 2.0f;
        }
        else
        {
            fallingDown = false;
            jumpingUp = true;
            movingEdge = false;
            velocity = heading * characterStats.moveSpeed / 3.0f;
            float difference = targetY - transform.position.y;
            velocity.y = characterStats.jumpVelocity * (0.5f + difference / 2.0f);
        }
    }

    // Maneja la caída del personaje
    void FallDownward(Vector3 target)
    {
        velocity += Physics.gravity * Time.deltaTime;

        if (transform.position.y <= target.y)
        {
            fallingDown = false;
            jumpingUp = false;
            movingEdge = false;

            Vector3 p = transform.position;
            p.y = target.y;
            transform.position = p;
            velocity = new Vector3();
        }
    }

    // Maneja el ascenso del personaje
    void JumpUpward(Vector3 target)
    {
        velocity += Physics.gravity * Time.deltaTime;

        if (transform.position.y > target.y)
        {
            jumpingUp = false;
            fallingDown = true;
        }
    }

    // Mueve al personaje hacia el borde
    void MoveToEdge()
    {
        if (Vector3.Distance(transform.position, jumpTarget) >= 0.05f)
        {
            SetHorizontalVelocity();
        }
        else
        {
            movingEdge = false;
            fallingDown = true;
            velocity /= 5.0f;
            velocity.y = 1.5f;
        }
    }

    // Encuentra el tile con el menor costo total (f)
    protected Tile FindLowestF(List<Tile> list)
    {
        Tile lowest = list[0];

        foreach (Tile t in list)
        {
            if (t.f < lowest.f)
            {
                lowest = t;
            }
        }

        list.Remove(lowest);
        return lowest;
    }

    // Encuentra el tile final en el camino
    protected Tile FindEndTile(Tile t)
    {
        Stack<Tile> tempPath = new Stack<Tile>();
        Tile next = t.parent;

        while (next != null)
        {
            tempPath.Push(next);
            next = next.parent;
        }

        if (tempPath.Count <= characterStats.move)
        {
            return t.parent;
        }

        Tile endTile = null;
        for (int i = 0; i <= characterStats.move; i++)
        {
            endTile = tempPath.Pop();
        }

        return endTile;
    }

    // Encuentra el camino hacia el tile objetivo usando A*
    public void FindPath(Tile target)
{
    ComputeAdjacencyLists(characterStats.jumpHeight, target);
    GetCurrentTile();

    List<Tile> openList = new List<Tile>();
    List<Tile> closedList = new List<Tile>();

    openList.Add(currentTile);
    currentTile.h = Vector3.Distance(currentTile.transform.position, target.transform.position);
    currentTile.f = currentTile.h;

    while (openList.Count > 0)
    {
        Tile t = FindLowestF(openList);
        closedList.Add(t);

        if (t == target)
        {
            actualTargetTile = FindEndTile(t);
            MoveToTile(actualTargetTile);
            return;
        }

        foreach (Tile tile in t.adjacencyList)
        {
            if (closedList.Contains(tile))
            {
                continue;
            }

            if (!openList.Contains(tile))
            {
                tile.parent = t;
                tile.g = t.g + Vector3.Distance(tile.transform.position, t.transform.position);
                float heightDifference = Mathf.Abs(tile.transform.position.y - t.transform.position.y);
                int additionalCost = (int)(heightDifference * 2); // Coste adicional por altura
                tile.g += additionalCost;
                tile.h = Vector3.Distance(tile.transform.position, target.transform.position);
                tile.f = tile.g + tile.h;
                openList.Add(tile);
            }
            else
            {
                float tempG = t.g + Vector3.Distance(tile.transform.position, t.transform.position);
                float heightDifference = Mathf.Abs(tile.transform.position.y - t.transform.position.y);
                int additionalCost = (int)(heightDifference * 2); // Coste adicional por altura
                tempG += additionalCost;

                if (tempG < tile.g)
                {
                    tile.parent = t;
                    tile.g = tempG;
                    tile.f = tile.g + tile.h;
                }
            }
        }
    }

        Debug.Log("Path not found");
    
    }
    

    
}


