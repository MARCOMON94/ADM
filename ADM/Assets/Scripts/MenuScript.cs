using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MenuScript 
{
    // Crea en la barra superior un menú "Tools" que ejecuta la acción de añadir el script Tile a todos los GameObjects con la etiqueta "Tile"
    [MenuItem("Tools/Assign Tile Script")]
    public static void AssignTileScript()
    {
        // Encuentra todos los GameObjects con la etiqueta "Tile"
        GameObject[] tiles = GameObject.FindGameObjectsWithTag("Tile");

        // Añade el componente Tile a cada uno de ellos
        foreach (GameObject t in tiles)
        {
            t.AddComponent<Tile>();
        }
    }
}
