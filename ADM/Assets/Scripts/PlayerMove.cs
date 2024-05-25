using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : TacticsMove
{
    // Método Start
    void Start()
    {
        Init(); // Llama al método Init de TacticsMove para inicializar
    }

    // Método Update
    void Update()
    {
        if (!turn)
        {
            return;
        }

        if (!moving)
        {
            FindSelectableTiles(); // Encuentra los tiles seleccionables

            CheckMouse(); // Revisa la entrada del ratón
        }
        else
        {
            Move(); // Llama al método Move para mover el personaje
        }
    }

    // Método para revisar la entrada del ratón
    void CheckMouse()
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
                        MoveToTile(t); // Mueve al personaje al tile seleccionado
                    }
                }
            }
        }
    }
}