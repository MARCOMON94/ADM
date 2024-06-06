using System;
using UnityEngine;

[Serializable]
public class CharacterStatsSnapshot
{
    public string name;
    public int move;
    public float jumpHeight;
    public float moveSpeed;
    public float jumpVelocity;
    public int speed;
    public int basicDamage;
    public float attackRange;
    public int health;
    public AttackType attackType;
    public bool heightAttack;
    public bool canFly;

    public CharacterStatsSnapshot(CharacterStatsSO stats)
    {
        name = stats.name;
        move = stats.move;
        jumpHeight = stats.jumpHeight;
        moveSpeed = stats.moveSpeed;
        jumpVelocity = stats.jumpVelocity;
        speed = stats.speed;
        basicDamage = stats.basicDamage;
        attackRange = stats.attackRange;
        health = stats.health;
        attackType = stats.attackType;
        heightAttack = stats.heightAttack;
        canFly = stats.canFly;
    }

    public void Restore(CharacterStatsSO stats)
    {
        stats.name = name;
        stats.move = move;
        stats.jumpHeight = jumpHeight;
        stats.moveSpeed = moveSpeed;
        stats.jumpVelocity = jumpVelocity;
        stats.speed = speed;
        stats.basicDamage = basicDamage;
        stats.attackRange = attackRange;
        stats.health = health;
        stats.attackType = attackType;
        stats.heightAttack = heightAttack;
        stats.canFly = canFly;
    }
}
