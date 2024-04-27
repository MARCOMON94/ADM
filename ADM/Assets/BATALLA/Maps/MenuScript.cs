using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class MenuScript 
{
    /* crea en la barra superior un tools, que hace que todos los elementos con la etiqueta que 
    le pongas ejecuten la acción que mandes, en este caso la de añadirle el scrip Tile*/
    
    [MenuItem("Tools/Assign Tile Script")]
    public static void AssignTileScript()
    {
        GameObject[] tiles = GameObject.FindGameObjectsWithTag("Tile");
        

        foreach (GameObject t in tiles)
        {
            t.AddComponent<Tile>();
        }
    }
}

