using System.Collections.Generic;
using UnityEngine;

public class SwordSlash : MonoBehaviour
{
    private float damage;
    private float knockbackForce;
    private float collisionDamage;
    private bool hitsTwice;

    private List<Collider> hitEnemies = new List<Collider>();

    public void Initialize(float dmg, float knockback, float colDmg, float rangeMultiplier, bool doubleHit)
    {
        damage = dmg;
        knockbackForce = knockback;
        collisionDamage = colDmg;
        hitsTwice = doubleHit;

        // Scale hitbox for the "Further Melee Range" upgrade
        transform.localScale = new Vector3(transform.localScale.x * rangeMultiplier, transform.localScale.y, transform.localScale.z * rangeMultiplier);

        Destroy(gameObject, 0.4f); // Auto destroy slash effect after animation
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy") && !hitEnemies.Contains(other))
        {
            hitEnemies.Add(other);
            EnemyBase enemy = other.GetComponentInParent<EnemyBase>();

            if (enemy != null)
            {
                int hitCount = hitsTwice ? 2 : 1;
                for (int i = 0; i < hitCount; i++)
                {
                    enemy.takeDamage(damage);
                }

                if (knockbackForce > 0f)
                {
                    Vector3 knockDir = (enemy.transform.position - transform.position).normalized;
                    knockDir.y = 0f; // Keep knockback along ground plane

                    KnockedEnemy knockedComp = enemy.gameObject.GetComponent<KnockedEnemy>();
                    if (knockedComp == null)
                    {
                        knockedComp = enemy.gameObject.AddComponent<KnockedEnemy>();
                    }

                    knockedComp.Launch(knockDir * knockbackForce, collisionDamage);
                }
            }
        }
    }
}