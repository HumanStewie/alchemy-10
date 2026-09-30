using UnityEngine;

public class SimpleFollower : EnemyBase
{
    [Header("Melee Hitbox Settings")]
    [SerializeField] private float hitboxRadius = 0.8f;
    [SerializeField] private float hitboxForwardOffset = 1.2f;
    [SerializeField] private LayerMask targetMask;

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
        if (currentTarget == null) return;

        float dist = Vector3.Distance(transform.position, currentTarget.position);

        if (dist > attackRange)
        {
            MoveTowards(currentTarget.position, currentspeed);
        }
        else if (attackCooldownTimer <= 0f)
        {
            PerformAttack();
            attackCooldownTimer = 2f;
        }
    }

    private void PerformAttack()
    {
        TriggerAttackAnimation();

        Vector3 forwardDir = transform.right;
        Vector3 hitboxCenter = transform.position + Vector3.up * 0.5f + (forwardDir * hitboxForwardOffset);

        Collider[] hits = Physics.OverlapSphere(hitboxCenter, hitboxRadius);

        for (int i = 0; i < hits.Length; i++)
        {
            Collider col = hits[i];
            if (col.transform == transform || col.transform.IsChildOf(transform)) continue;

            if (col.CompareTag("Player") || col.TryGetComponent<PlayerHealthAndStat>(out var _))
            {
                var health = col.GetComponentInParent<PlayerHealthAndStat>();
                if (health != null)
                {
                    health.takeDamage(damage);
                    break;
                }
            }

            if (col.TryGetComponent<RuneManager>(out var rune))
            {
                rune.currentHealth = Mathf.Max(0f, rune.currentHealth - damage);
                break;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector3 forwardDir = Application.isPlaying ? transform.right : transform.forward;
        Vector3 hitboxCenter = transform.position + Vector3.up * 0.5f + (forwardDir * hitboxForwardOffset);
        Gizmos.DrawWireSphere(hitboxCenter, hitboxRadius);
    }
}