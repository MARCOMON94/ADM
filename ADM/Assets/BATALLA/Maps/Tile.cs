using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{

    // Variables para determinar el estado del Tile
    public bool walkable = true;    // Si es transitable
    public bool current = false;    // Si es la posición actual del jugador
    public bool target = false;     // Si es el destino del jugador
    public bool selectable = false; // Si es seleccionable



    // Lista para almacenar los tiles adyacentes
    public List<Tile> adjacencyList = new List<Tile>();



    // Variables para la búsqueda de caminos
    public bool visited = false;    // Si el tile ha sido visitado
    public Tile parent = null;      // El tile padre en el camino
    public int distance = 0;        // Distancia desde el inicio

    //For A*

    public float f = 0;
    public float g = 0;
    public float h = 0;

    // Método Update para cambiar el color del tile según su estado
    void Update()
    {
        if (current)
        {
            GetComponent<Renderer>().material.color = Color.magenta; // Color para el tile actual
        }
        else if (target)
        {
            GetComponent<Renderer>().material.color = Color.green;   // Color para el tile objetivo
        }
        else if (selectable)
        {
            GetComponent<Renderer>().material.color = Color.red;     // Color para el tile seleccionable
        }
        else 
        {
            GetComponent<Renderer>().material.color = Color.white;   // Color por defecto
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

        f = g = h = 0;
    }



    // Encuentra los vecinos del tile que son transitables
    public void FindNeighbors(float jumpHeight, Tile target)
    {
        Reset();

        // Revisa los tiles en las direcciones especificadas
        CheckTile(Vector3.forward, jumpHeight, target);
        CheckTile(-Vector3.forward, jumpHeight, target);
        CheckTile(Vector3.right, jumpHeight, target);
        CheckTile(-Vector3.right, jumpHeight, target);
    }
    //Encuentra los tiles vecinos transitables. Llama a Reset para asegurarse de que el tile esté en su estado inicial y luego revisa en las cuatro direcciones cardinales.




    // Revisa si hay un tile transitable en la dirección dada
    public void CheckTile(Vector3 direction, float jumpHeight, Tile target)
    {
        Vector3 halfExtents = new Vector3(0.25f, (1 + jumpHeight) / 2.0f, 0.25f);
        Collider[] colliders = Physics.OverlapBox(transform.position + direction, halfExtents);

        foreach (Collider item in colliders)
        {
            Tile tile = item.GetComponent<Tile>();
            if (tile != null && tile.walkable)
            {
                RaycastHit hit;
                if (!Physics.Raycast(tile.transform.position, Vector3.up, out hit, 1) || (tile == target))
                {
                    adjacencyList.Add(tile); // Añade el tile a la lista de adyacentes si es transitable
                }
            }
        }
    }
}

/*Futuro Marco notas: con el renderer, será donde los bloques estarán con la opacidad a 0 y luego los que se puedan seleccionar los pondre con una 
opacidad un poco más alta, para que solo se vean los bloques a los uqe se pueden mover.

Además comprobar si en esta página es dónde quiero poner que las casillas horizontales disminuyan si se puede llegar o no, probalblemente habrá que
modificar el método checktile porque ahora mismo funciona con el jumpHeight*/

