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

        transform.localScale = new Vector3(transform.localScale.x * rangeMultiplier, transform.localScale.y, transform.localScale.z * rangeMultiplier);

        CheckInstantOverlap();

        Destroy(gameObject, 0.4f);
    }

    private void CheckInstantOverlap()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, 1.2f * transform.localScale.x);
        for (int i = 0; i < hits.Length; i++)
        {
            TryDamageTarget(hits[i]);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        TryDamageTarget(other);
    }

    private void TryDamageTarget(Collider other)
    {
        if (other == null || other.CompareTag("Player") || hitEnemies.Contains(other)) return;

        if (other.CompareTag("Enemy") || other.GetComponentInParent<EnemyBase>() != null)
        {
            hitEnemies.Add(other);

            int hitCount = hitsTwice ? 2 : 1;
            for (int i = 0; i < hitCount; i++)
            {
                other.SendMessageUpwards("takeDamage", damage, SendMessageOptions.DontRequireReceiver);
            }

            if (knockbackForce > 0f)
            {
                Vector3 knockDir = (other.transform.position - transform.position).normalized;
                knockDir.y = 0f;

                KnockedEnemy knockedComp = other.GetComponentInParent<KnockedEnemy>();
                if (knockedComp == null)
                {
                    knockedComp = other.gameObject.AddComponent<KnockedEnemy>();
                }

                knockedComp.Launch(knockDir * knockbackForce, collisionDamage);
            }
        }
    }
}