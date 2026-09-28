using UnityEngine;

public class SwarmFollower : SimpleFollower
{
    protected override void Start()
    {
        base.Start();

        maxHP = 1f;
        currentHP = 1f;
        damage = 3f;
        moveSpeed = 5.5f; 
        currentspeed = moveSpeed;
        attackRange = 1.2f; 
        transform.localScale = Vector3.one * 0.3f;
    }
}