using DG.Tweening;
using System.Collections;
using UnityEngine;

public class SimpleFollower : EnemyBase
{
    [Header("Melee Hitbox Settings")]
    [SerializeField] private float hitboxRadius = 0.8f;
    [SerializeField] private float hitboxForwardOffset = 1.2f;
    [SerializeField] private LayerMask targetMask;

    [Header("Windup & Jump Attack")]
    [SerializeField] private float windupDuration = 0.5f;
    [SerializeField] private float jumpPower = 0.6f;
    [SerializeField] private float jumpDuration = 0.28f;
    [SerializeField] private float leapForwardDistance = 0.8f;

    private bool isAttacking = false;

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
        if (currentTarget == null || isAttacking) return;

        float dist = Vector3.Distance(transform.position, currentTarget.position);

        if (dist > attackRange)
        {
            MoveTowards(currentTarget.position, currentspeed);
        }
        else if (attackCooldownTimer <= 0f)
        {
            StartCoroutine(AttackRoutine());
            attackCooldownTimer = 2f;
        }
    }

    private IEnumerator AttackRoutine()
    {
        isAttacking = true;

        if (enemyAnimator != null)
        {
            enemyAnimator.SetBool(IsMovingHash, false);
        }

        float elapsed = 0f;
        while (elapsed < windupDuration)
        {
            elapsed += Time.deltaTime;

            if (currentTarget != null)
            {
                Vector3 lookDir = (currentTarget.position - transform.position);
                lookDir.y = 0f;
                if (lookDir.sqrMagnitude > 0.01f)
                {
                    Quaternion targetRot = Quaternion.LookRotation(lookDir) * Quaternion.Euler(0f, 90f, 0f);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, 12f * Time.deltaTime);
                }
            }

            yield return null;
        }

        TriggerAttackAnimation();

        Vector3 forwardDir = transform.right;
        Vector3 jumpTarget = transform.position + forwardDir * leapForwardDistance;

        if (Physics.Raycast(jumpTarget + Vector3.up * 1f, Vector3.down, out RaycastHit groundHit, 3f))
        {
            jumpTarget.y = groundHit.point.y;
        }

        Tween jumpTween = transform.DOJump(jumpTarget, jumpPower, 1, jumpDuration).SetEase(Ease.OutQuad);

        yield return new WaitForSeconds(jumpDuration * 0.5f);
        CheckHitbox();

        yield return jumpTween.WaitForCompletion();

        isAttacking = false;
    }

    private void CheckHitbox()
    {
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

    protected override void OnDisable()
    {
        base.OnDisable();
        transform.DOKill();
        isAttacking = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector3 forwardDir = Application.isPlaying ? transform.right : transform.forward;
        Vector3 hitboxCenter = transform.position + Vector3.up * 0.5f + (forwardDir * hitboxForwardOffset);
        Gizmos.DrawWireSphere(hitboxCenter, hitboxRadius);
    }
}