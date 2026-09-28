using UnityEngine;

public class SimpleFollower : EnemyBase
{
    protected override void Start()
    {
        maxHP = 7f;
        damage = 5f;
        moveSpeed = 5f;
        attackRange = 1.8f;
        preferRune = true;
        base.Start();
    }

    protected override void BehaviorUpdate()
    {
        float dist = Vector3.Distance(transform.position, currentTarget.position);

        if (dist > attackRange)
        {
            MoveTowards(currentTarget.position, currentspeed);
        }
        else if (attackCooldownTimer <= 0f)
        {
            if (currentTarget.CompareTag("Player"))
            {
                var health = currentTarget.GetComponent<PlayerHealthAndStat>();
                if (health != null) health.takeDamage(damage);
            }

            attackCooldownTimer = 2f;
        }
    }
}