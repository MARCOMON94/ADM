using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManagement : MonoBehaviour
{
    public GameObject personaje; // Asegúrate de tener esta variable definida en tu clase.
    public int movimientoDisponible = 3; // Movimiento total que puede realizar el personaje.

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Detecta clic del mouse
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                Square goalSquare = hit.collider.GetComponent<Square>();
                if (goalSquare != null)
                {
                    Vector3 posicionActual = personaje.transform.position;
                    Vector3 posicionDestino = goalSquare.transform.position;
                    // Calcula si el movimiento es diagonal.
                    bool esDiagonal = Mathf.Abs(posicionDestino.x - posicionActual.x) > 0 && Mathf.Abs(posicionDestino.z - posicionActual.z) > 0;
                    int costeMovimiento = esDiagonal ? 2 : 1; // Coste doble si es diagonal.
                    if (costeMovimiento <= movimientoDisponible)
                    {
                        // Mueve el personaje al centro de la casilla.
                        personaje.transform.position = posicionDestino;
                        movimientoDisponible -= costeMovimiento; // Actualiza el movimiento disponible.
                    }
                }
            }
        }
    }
}