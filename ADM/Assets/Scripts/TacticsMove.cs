using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TacticsMove : MonoBehaviour
{
    // Indica si es el turno del personaje
    public bool turn = false;

    // Lista de tiles seleccionables
    protected List<Tile> selectableTiles = new List<Tile>();

    // Array de todos los tiles en el mapa
    GameObject[] tiles;

    // Pila que representa el camino a seguir
    Stack<Tile> path = new Stack<Tile>();

    // Tile actual en el que se encuentra el personaje
    Tile currentTile;

    // Indica si el personaje está en movimiento
    public bool moving = false;

    // Usar ScriptableObject para estadísticas del personaje
    public CharacterStatsSO characterStats;

    // Velocidad actual del personaje
    Vector3 velocity = new Vector3();

    // Dirección del movimiento
    Vector3 heading = new Vector3();

    // Mitad de la altura del colisionador del personaje
    float halfHeight = 0;

    // Indicadores de estado de movimiento
    bool fallingDown = false;
    bool jumpingUp = false;
    bool movingEdge = false;

    // Objetivo del salto
    Vector3 jumpTarget;

    // Tile objetivo final
    public Tile actualTargetTile;
    

    // Método de inicialización
    protected void Init(bool isPlayer)
    {
        // Encuentra todos los tiles en el mapa
        tiles = GameObject.FindGameObjectsWithTag("Tile");

        // Obtiene la mitad de la altura del colisionador
        halfHeight = GetComponent<Collider>().bounds.extents.y;

        // Añade este personaje al TurnManager
        TurnManager.AddUnit(this);
    }

    // Método para iniciar el turno del personaje
    public virtual void BeginTurn()
    {
        // Marca el turno como activo
        turn = true;
    }

    // Método para finalizar el turno del personaje
    public virtual void EndTurn()
    {
        // Marca el turno como inactivo
        turn = false;
        RemoveSelectableTiles();

        // Notifica al TurnManager para finalizar el turno solo si no es el NPC quien llama directamente a EndTurn
        if (!(this is NPCMove))
        {
            TurnManager.EndTurn();
        }
    }

    // Obtiene el tile actual en el que se encuentra el personaje
    public void GetCurrentTile()
    {
        // Obtiene el tile debajo del personaje
        currentTile = GetTargetTile(gameObject);
        // Marca el tile como actual
        currentTile.current = true;
    }

    // Obtiene el tile debajo de un GameObject específico
    public Tile GetTargetTile(GameObject target)
    {
        RaycastHit hit;
        Tile tile = null;

        // Lanza un rayo hacia abajo desde la posición del target
        if (Physics.Raycast(target.transform.position, -Vector3.up, out hit, 1))
        {
            // Obtiene el componente Tile del objeto golpeado
            tile = hit.collider.GetComponent<Tile>();
        }
        return tile;
    }

    // Calcula la lista de tiles adyacentes teniendo en cuenta la altura de salto
    public void ComputeAdjacencyLists(float jumpHeight, Tile target, bool ignoreOccupied)
{
    foreach (GameObject tile in tiles)
    {
        Tile t = tile.GetComponent<Tile>();
        t.FindNeighbors(jumpHeight, target, ignoreOccupied); // Pasar el parámetro ignoreOccupied
    }
}


    // Encuentra los tiles seleccionables dentro del rango de movimiento
    public void FindSelectableTiles(bool ignoreOccupied = false)
{
    ComputeAdjacencyLists(characterStats.jumpHeight, null, ignoreOccupied);
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

                    float heightDifference = Mathf.Abs(tile.transform.position.y - t.transform.position.y);
                    int additionalCost = (int)(heightDifference * 2);
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
        // Limpia el camino actual
        path.Clear();
        // Marca el tile como objetivo
        tile.target = true;
        // Marca que el personaje está en movimiento
        moving = true;

        Tile next = tile;
        while (next != null) // Recorre el camino hasta el tile objetivo
        {
            // Añade cada tile al camino
            path.Push(next);
            // Sigue al tile padre
            next = next.parent;
        }
    }

    // Método para mover al personaje
    public void Move()
    {
        if (path.Count > 0) // Si hay tiles en el camino
        {
            // Obtiene el siguiente tile en el camino
            Tile t = path.Peek();
            Vector3 target = t.transform.position;

            // Ajusta la posición de destino para tener en cuenta la altura del colisionador
            target.y += halfHeight + t.GetComponent<Collider>().bounds.extents.y;

            if (Vector3.Distance(transform.position, target) >= 0.05f) // Si el personaje no ha llegado al tile objetivo
            {
                bool jump = transform.position.y != target.y;

                if (jump)
                {
                    // Si hay diferencia de altura, realiza un salto
                    Jump(target);
                }
                else
                {
                    // Calcula la dirección hacia el objetivo y establece la velocidad horizontal
                    CalculateHeading(target);
                    SetHorizontalVelocity();
                }

                // Establece la dirección del personaje
                transform.forward = heading;
                // Mueve al personaje
                transform.position += velocity * Time.deltaTime;
            }
            else
            {
                // Establece la posición final del personaje
                transform.position = target;
                // Elimina el tile alcanzado del camino
                path.Pop();
                // Resetea los estados de movimiento
                ResetMovementStates();
            }
        }
        else
        {
            // Elimina los tiles seleccionables y marca que el personaje ha dejado de moverse
            RemoveSelectableTiles();
            moving = false;
            // Resetea los estados de movimiento
            ResetMovementStates();
            // Notifica al TurnManager para finalizar el turno
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
    void RemoveSelectableTiles()
    {
        if (currentTile != null)
        {
            currentTile.current = false;
            currentTile = null;
        }

        foreach (Tile tile in selectableTiles)
        {
            // Resetea el estado del tile
            tile.Reset();
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
            // Maneja la caída
            FallDownward(target);
        }
        else if (jumpingUp)
        {
            // Maneja el ascenso
            JumpUpward(target);
        }
        else if (movingEdge)
        {
            // Mueve al personaje hacia el borde
            MoveToEdge();
        }
        else
        {
            // Prepara el salto
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
    // Pasar false como valor por defecto para ignoreOccupied
    ComputeAdjacencyLists(characterStats.jumpHeight, target, false);
    GetCurrentTile();

    List<Tile> openList = new List<Tile>(); // Lista de tiles por evaluar
    List<Tile> closedList = new List<Tile>(); // Lista de tiles ya evaluados

    // Añade el tile actual a la lista de tiles por evaluar
    openList.Add(currentTile);
    // Calcula la heurística del tile actual
    currentTile.h = Vector3.Distance(currentTile.transform.position, target.transform.position);
    // Establece el costo total del tile actual
    currentTile.f = currentTile.h;

    while (openList.Count > 0)
    {
        // Encuentra el tile con el menor costo total
        Tile t = FindLowestF(openList);
        // Añade el tile a la lista de tiles evaluados
        closedList.Add(t);

        if (t == target) // Si el tile actual es el objetivo
        {
            // Encuentra el tile final en el camino
            actualTargetTile = FindEndTile(t);
            // Mueve al personaje al tile objetivo
            MoveToTile(actualTargetTile);
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
                // Establece el tile actual como el padre del tile adyacente
                tile.parent = t;
                // Calcula el costo desde el inicio hasta el nodo actual
                tile.g = t.g + Vector3.Distance(tile.transform.position, t.transform.position);
                // Calcula la diferencia de altura entre los tiles
                float heightDifference = Mathf.Abs(tile.transform.position.y - t.transform.position.y);
                // Coste adicional por altura
                int additionalCost = (int)(heightDifference * 2);
                tile.g += additionalCost;
                // Calcula la heurística
                tile.h = Vector3.Distance(tile.transform.position, target.transform.position);
                // Establece el costo total
                tile.f = tile.g + tile.h;
                // Añade el tile a la lista de tiles por evaluar
                openList.Add(tile);
            }
            else // Si el tile ya está en la lista de tiles por evaluar
            {
                // Calcula el costo temporal
                float tempG = t.g + Vector3.Distance(tile.transform.position, t.transform.position);
                // Calcula la diferencia de altura entre los tiles
                float heightDifference = Mathf.Abs(tile.transform.position.y - t.transform.position.y);
                // Coste adicional por altura
                int additionalCost = (int)(heightDifference * 2);
                tempG += additionalCost;

                if (tempG < tile.g) // Si el costo temporal es menor que el costo actual
                {
                    // Actualiza el tile padre
                    tile.parent = t;
                    // Actualiza el costo desde el inicio
                    tile.g = tempG;
                    // Actualiza el costo total
                    tile.f = tile.g + tile.h;
                }
            }
        }
    }

    Debug.Log("Path not found");
}

    // Encuentra los tiles en el rango de ataque
    public void FindAttackableTiles()
{
    // Pasar true para ignoreOccupied, ya que estamos buscando tiles en rango de ataque
    ComputeAdjacencyLists(characterStats.jumpHeight, null, true);
    GetCurrentTile();

    Queue<Tile> process = new Queue<Tile>();
    // Añade el tile actual a la cola de procesamiento
    process.Enqueue(currentTile);
    // Marca el tile actual como visitado
    currentTile.visited = true;

    while (process.Count > 0)
    {
        // Toma el siguiente tile de la cola
        Tile t = process.Dequeue();
        // Añade el tile a la lista de tiles seleccionables
        selectableTiles.Add(t);
        // Marca el tile como seleccionable
        t.selectable = true;

        if (t.distance < characterStats.attackRange) // Si la distancia del tile es menor que el rango de ataque del personaje
        {
            foreach (Tile tile in t.adjacencyList) // Revisa cada tile adyacente
            {
                if (!tile.visited) // Si el tile no ha sido visitado
                {
                    // Establece el tile actual como el padre del tile adyacente
                    tile.parent = t;
                    // Marca el tile adyacente como visitado
                    tile.visited = true;
                    // Establece la distancia del tile adyacente
                    tile.distance = 1 + t.distance;
                    // Añade el tile adyacente a la cola de procesamiento
                    process.Enqueue(tile);
                }
            }
        }
    }
}
    //FALTA AÑADIR
    public virtual void UpdateHealthUI()
{
    // Método virtual para ser sobrescrito por clases derivadas
}

}
