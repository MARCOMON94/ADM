using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    // Variables para determinar el estado del Tile
    public bool walkable = true; // Indica si el tile es transitable
    public bool current = false; // Indica si el tile es el actual
    public bool target = false; // Indica si el tile es el objetivo
    public bool selectable = false; // Indica si el tile es seleccionable

    // Lista para almacenar los tiles adyacentes
    public List<Tile> adjacencyList = new List<Tile>();

    // Variables para la búsqueda de caminos
    public bool visited = false; // Indica si el tile ha sido visitado
    public Tile parent = null; // Tile padre en la ruta
    public int distance = 0; // Distancia desde el inicio

    // Variables adicionales para A*
    public float f = 0; // Costo total (g + h)
    public float g = 0; // Costo desde el inicio hasta el nodo actual
    public float h = 0; // Heurística (estimación del costo desde el nodo actual hasta el objetivo)
    public bool ignoreOccupied = false;

    // Método Update que se llama una vez por frame
    void Update()
    {
        // Cambia el color del tile basado en su estado
        if (current)
        {
            GetComponent<Renderer>().material.color = Color.magenta; // Color para el tile actual
        }
        else if (target)
        {
            GetComponent<Renderer>().material.color = Color.green; // Color para el tile objetivo
        }
        else if (selectable)
        {
            GetComponent<Renderer>().material.color = Color.red; // Color para el tile seleccionable
        }
        else
        {
            GetComponent<Renderer>().material.color = Color.white; // Color por defecto
        }
    }

    // Resetea el estado del tile
    public void Reset()
    {
        adjacencyList.Clear();
        current = false;
        target = false;
        selectable = false;
        visited = false;
        parent = null;
        distance = 0;
        f = g = h = 0; // Resetea los valores para A*
    }

    // Encuentra los vecinos del tile que son transitables
    public void FindNeighbors(float jumpHeight, Tile target, bool ignoreOccupied)
{
    Reset();

    CheckTile(Vector3.forward, jumpHeight, target, ignoreOccupied);
    CheckTile(-Vector3.forward, jumpHeight, target, ignoreOccupied);
    CheckTile(Vector3.right, jumpHeight, target, ignoreOccupied);
    CheckTile(-Vector3.right, jumpHeight, target, ignoreOccupied);
}

    // Revisa si hay un tile transitable en la dirección dada
    public void CheckTile(Vector3 direction, float jumpHeight, Tile target, bool ignoreOccupied)
{
    Vector3 halfExtents = new Vector3(0.25f, (1 + jumpHeight) / 2.0f, 0.25f);
    Collider[] colliders = Physics.OverlapBox(transform.position + direction, halfExtents);

    foreach (Collider item in colliders)
    {
        Tile tile = item.GetComponent<Tile>();
        if (tile != null && tile.walkable)
        {
            RaycastHit hit;
            // Modificar la condición para ignorar la ocupación si ignoreOccupied es true
            if (ignoreOccupied || !Physics.Raycast(tile.transform.position, Vector3.up, out hit, 1) || (tile == target))
            {
                adjacencyList.Add(tile);
                Debug.Log($"Tile {tile.name} es transitable y se ha añadido a la lista de adyacencia.");
            }
            else
            {
                Debug.Log($"Tile {tile.name} no es transitable porque está ocupado.");
            }
        }
        else if (tile != null)
        {
            Debug.Log($"Tile {tile.name} no es transitable porque walkable es falso.");
        }
    }
}
}
