using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterStats", menuName = "Character Stats", order = 51)]
public class CharacterStatsSO : ScriptableObject
{
    public string name = "Character";
    public int move = 5;
    public float jumpHeight = 2;
    public float moveSpeed = 2;
    public float jumpVelocity = 4.5f;
    public int speed = 5; // Velocidad del personaje
    public int basicDamage = 10; // Daño básico
    public float attackRange = 1.5f; // Distancia de ataque
    public int health = 100; // Vida
}

