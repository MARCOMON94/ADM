using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TacticsMove : MonoBehaviour
{
    public bool turn = false; // Indica si es el turno del personaje
    protected List<Tile> selectableTiles = new List<Tile>(); // Lista de tiles seleccionables
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
    protected void Init(bool isPlayer)
    {
        tiles = GameObject.FindGameObjectsWithTag("Tile"); // Encuentra todos los tiles en el mapa
        halfHeight = GetComponent<Collider>().bounds.extents.y; // Obtiene la mitad de la altura del colisionador
        TurnManager.AddUnit(this, isPlayer); // Añade este personaje al TurnManager
    }

    // Método para iniciar el turno del personaje
    public void BeginTurn()
    {
        turn = true; // Marca el turno como activo
    }

    // Método para finalizar el turno del personaje
    public void EndTurn()
    {
        turn = false; // Marca el turno como inactivo
    }

    // Obtiene el tile actual en el que se encuentra el personaje
    public void GetCurrentTile()
    {
        currentTile = GetTargetTile(gameObject); // Obtiene el tile debajo del personaje
        currentTile.current = true; // Marca el tile como actual
    }

    // Obtiene el tile debajo de un GameObject específico
    public Tile GetTargetTile(GameObject target)
    {
        RaycastHit hit;
        Tile tile = null;

        // Lanza un rayo hacia abajo desde la posición del target
        if (Physics.Raycast(target.transform.position, -Vector3.up, out hit, 1))
        {
            tile = hit.collider.GetComponent<Tile>(); // Obtiene el componente Tile del objeto golpeado
        }
        return tile;
    }

    // Calcula la lista de tiles adyacentes teniendo en cuenta la altura de salto
    public void ComputeAdjacencyLists(float jumpHeight, Tile target)
    {
        foreach (GameObject tile in tiles)
        {
            Tile t = tile.GetComponent<Tile>();
            t.FindNeighbors(jumpHeight, target); // Encuentra los vecinos del tile actual
        }
    }

    // Encuentra los tiles seleccionables dentro del rango de movimiento
    public void FindSelectableTiles()
{
    ComputeAdjacencyLists(characterStats.jumpHeight, null); // Calcula la lista de tiles adyacentes
    GetCurrentTile(); // Obtiene el tile actual

    Queue<Tile> process = new Queue<Tile>();
    process.Enqueue(currentTile); // Añade el tile actual a la cola de procesamiento
    currentTile.visited = true; // Marca el tile actual como visitado

    while (process.Count > 0)
    {
        Tile t = process.Dequeue(); // Toma el siguiente tile de la cola
        selectableTiles.Add(t); // Añade el tile a la lista de tiles seleccionables
        t.selectable = true; // Marca el tile como seleccionable

        if (t.distance < characterStats.move) // Si la distancia del tile es menor que el rango de movimiento del personaje
        {
            foreach (Tile tile in t.adjacencyList) // Revisa cada tile adyacente
            {
                if (!tile.visited) // Si el tile no ha sido visitado
                {
                    tile.parent = t; // Establece el tile actual como el padre del tile adyacente
                    tile.visited = true; // Marca el tile adyacente como visitado

                    float heightDifference = Mathf.Abs(tile.transform.position.y - t.transform.position.y);
                    int additionalCost = (int)(heightDifference * 2); // Cada unidad de altura cuesta 2 unidades de movimiento
                    int newDistance = t.distance + 1 + additionalCost;

                    if (newDistance <= characterStats.move) // Si la nueva distancia es menor o igual al rango de movimiento del personaje
                    {
                        tile.distance = newDistance; // Establece la nueva distancia
                        process.Enqueue(tile); // Añade el tile adyacente a la cola de procesamiento
                    }
                }
            }
        }
    }
}

    // Mueve al personaje hacia un tile específico
    public void MoveToTile(Tile tile)
    {
        path.Clear(); // Limpia el camino actual
        tile.target = true; // Marca el tile como objetivo
        moving = true; // Marca que el personaje está en movimiento

        Tile next = tile;
        while (next != null) // Recorre el camino hasta el tile objetivo
        {
            path.Push(next); // Añade cada tile al camino
            next = next.parent; // Sigue al tile padre
        }
    }

    // Método para mover al personaje
    public void Move()
    {
        if (path.Count > 0) // Si hay tiles en el camino
        {
            Tile t = path.Peek(); // Obtiene el siguiente tile en el camino
            Vector3 target = t.transform.position;

            target.y += halfHeight + t.GetComponent<Collider>().bounds.extents.y;

            if (Vector3.Distance(transform.position, target) >= 0.05f) // Si el personaje no ha llegado al tile objetivo
            {
                bool jump = transform.position.y != target.y;

                if (jump)
                {
                    Jump(target); // Si hay diferencia de altura, realiza un salto
                }
                else
                {
                    CalculateHeading(target); // Calcula la dirección hacia el objetivo
                    SetHorizontalVelocity(); // Establece la velocidad horizontal
                }

                transform.forward = heading; // Establece la dirección del personaje
                transform.position += velocity * Time.deltaTime; // Mueve al personaje
            }
            else
            {
                transform.position = target; // Establece la posición final del personaje
                path.Pop(); // Elimina el tile alcanzado del camino
                ResetMovementStates(); // Resetea los estados de movimiento
            }
        }
        else
        {
            RemoveSelectableTiles(); // Elimina los tiles seleccionables
            moving = false; // Marca que el personaje ha dejado de moverse
            ResetMovementStates(); // Resetea los estados de movimiento
            TurnManager.EndTurn(); // Notifica al TurnManager para finalizar el turno
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
    void RemoveSelectableTiles()
    {
        if (currentTile != null)
        {
            currentTile.current = false;
            currentTile = null;
        }

        foreach (Tile tile in selectableTiles)
        {
            tile.Reset(); // Resetea el estado del tile
        }

        selectableTiles.Clear();
    }

    // Calcula la dirección hacia el objetivo
    void CalculateHeading(Vector3 target)
    {
        heading = target - transform.position; // Calcula la dirección hacia el objetivo
        heading.Normalize(); // Normaliza la dirección
    }

    // Establece la velocidad horizontal
    void SetHorizontalVelocity()
    {
        velocity = heading * characterStats.moveSpeed; // Establece la velocidad horizontal basada en la dirección y la velocidad de movimiento del personaje
    }

    // Método para manejar el salto
    void Jump(Vector3 target)
    {
        if (fallingDown)
        {
            FallDownward(target); // Maneja la caída
        }
        else if (jumpingUp)
        {
            JumpUpward(target); // Maneja el ascenso
        }
        else if (movingEdge)
        {
            MoveToEdge(); // Mueve al personaje hacia el borde
        }
        else
        {
            PrepareJump(target); // Prepara el salto
        }
    }

    // Prepara el salto
    void PrepareJump(Vector3 target)
    {
        float targetY = target.y; // Guarda el valor original de target.y
        Vector3 localTarget = target; // Crea una copia local de target para modificar
        localTarget.y = transform.position.y;

        CalculateHeading(localTarget); // Usa la copia local para los cálculos

        if (transform.position.y > targetY) // Si el personaje está más alto que el objetivo
        {
            fallingDown = false;
            jumpingUp = false;
            movingEdge = true;
            jumpTarget = transform.position + (localTarget - transform.position) / 2.0f; // Establece el objetivo del salto
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

        if (transform.position.y <= target.y) // Si el personaje ha alcanzado el objetivo
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

        if (transform.position.y > target.y) // Si el personaje está más alto que el objetivo
        {
            jumpingUp = false;
            fallingDown = true;
        }
    }

    // Mueve al personaje hacia el borde
    void MoveToEdge()
    {
        if (Vector3.Distance(transform.position, jumpTarget) >= 0.05f) // Si el personaje no ha alcanzado el borde
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
        ComputeAdjacencyLists(characterStats.jumpHeight, target); // Calcula la lista de tiles adyacentes
        GetCurrentTile(); // Obtiene el tile actual

        List<Tile> openList = new List<Tile>(); // Lista de tiles por evaluar
        List<Tile> closedList = new List<Tile>(); // Lista de tiles ya evaluados

        openList.Add(currentTile); // Añade el tile actual a la lista de tiles por evaluar
        currentTile.h = Vector3.Distance(currentTile.transform.position, target.transform.position); // Calcula la heurística del tile actual
        currentTile.f = currentTile.h; // Establece el costo total del tile actual

        while (openList.Count > 0)
        {
            Tile t = FindLowestF(openList); // Encuentra el tile con el menor costo total
            closedList.Add(t); // Añade el tile a la lista de tiles evaluados

            if (t == target) // Si el tile actual es el objetivo
            {
                actualTargetTile = FindEndTile(t); // Encuentra el tile final en el camino
                MoveToTile(actualTargetTile); // Mueve al personaje al tile objetivo
                return;
            }

            foreach (Tile tile in t.adjacencyList) // Revisa cada tile adyacente
            {
                if (closedList.Contains(tile)) // Si el tile ya ha sido evaluado, se salta
                {
                    continue;
                }

                if (!openList.Contains(tile)) // Si el tile no está en la lista de tiles por evaluar
                {
                    tile.parent = t; // Establece el tile actual como el padre del tile adyacente
                    tile.g = t.g + Vector3.Distance(tile.transform.position, t.transform.position); // Calcula el costo desde el inicio hasta el nodo actual
                    float heightDifference = Mathf.Abs(tile.transform.position.y - t.transform.position.y);
                    int additionalCost = (int)(heightDifference * 2); // Coste adicional por altura
                    tile.g += additionalCost;
                    tile.h = Vector3.Distance(tile.transform.position, target.transform.position); // Calcula la heurística
                    tile.f = tile.g + tile.h; // Establece el costo total
                    openList.Add(tile); // Añade el tile a la lista de tiles por evaluar
                }
                else // Si el tile ya está en la lista de tiles por evaluar
                {
                    float tempG = t.g + Vector3.Distance(tile.transform.position, t.transform.position); // Calcula el costo temporal
                    float heightDifference = Mathf.Abs(tile.transform.position.y - t.transform.position.y);
                    int additionalCost = (int)(heightDifference * 2); // Coste adicional por altura
                    tempG += additionalCost;

                    if (tempG < tile.g) // Si el costo temporal es menor que el costo actual
                    {
                        tile.parent = t; // Actualiza el tile padre
                        tile.g = tempG; // Actualiza el costo desde el inicio
                        tile.f = tile.g + tile.h; // Actualiza el costo total
                    }
                }
            }
        }

        Debug.Log("Path not found");
    }

    // Encuentra los tiles en el rango de ataque
    public void FindAttackableTiles()
    {
        ComputeAdjacencyLists(characterStats.jumpHeight, null); // Calcula la lista de tiles adyacentes
        GetCurrentTile(); // Obtiene el tile actual

        Queue<Tile> process = new Queue<Tile>();
        process.Enqueue(currentTile); // Añade el tile actual a la cola de procesamiento
        currentTile.visited = true; // Marca el tile actual como visitado

        while (process.Count > 0)
        {
            Tile t = process.Dequeue(); // Toma el siguiente tile de la cola
            selectableTiles.Add(t); // Añade el tile a la lista de tiles seleccionables
            t.selectable = true; // Marca el tile como seleccionable

            if (t.distance < characterStats.attackRange) // Si la distancia del tile es menor que el rango de ataque del personaje
            {
                foreach (Tile tile in t.adjacencyList) // Revisa cada tile adyacente
                {
                    if (!tile.visited) // Si el tile no ha sido visitado
                    {
                        tile.parent = t; // Establece el tile actual como el padre del tile adyacente
                        tile.visited = true; // Marca el tile adyacente como visitado
                        tile.distance = 1 + t.distance; // Establece la distancia del tile adyacente
                        process.Enqueue(tile); // Añade el tile adyacente a la cola de procesamiento
                    }
                }
            }
        }
    }
}
