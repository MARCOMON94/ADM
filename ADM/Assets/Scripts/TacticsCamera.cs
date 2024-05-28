using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TacticsCamera : MonoBehaviour
{
    // Un pivote para que la cámara no se desconfigure al cambiarle la posición, para quitar la que venía por defecto en el tutorial.
    public Transform cameraPivot;

    // Método para rotar la cámara hacia la izquierda
    public void RotateLeft()
    {
        if (cameraPivot != null)
        {
            cameraPivot.Rotate(Vector3.up, 90, Space.World);
        }
    }

    // Método para rotar la cámara hacia la derecha
    public void RotateRight()
    {
        if (cameraPivot != null)
        {
            cameraPivot.Rotate(Vector3.up, -90, Space.World);
        }
    }
}
