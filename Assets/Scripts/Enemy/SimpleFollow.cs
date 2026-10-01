using DG.Tweening;
using System.Collections;
using UnityEngine;

public class SimpleFollower : EnemyBase
{
    [Header("Melee Hitbox Settings")]
    [SerializeField] private float hitboxRadius = 1.0f;
    [SerializeField] private float hitboxForwardOffset = 1.2f;
    [SerializeField] private LayerMask targetMask;

    [Header("Slap Attack Settings")]
    [SerializeField] private float windupDuration = 0.45f;
    [SerializeField] private float slapDuration = 0.2f;

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
            StartCoroutine(SlapAttackRoutine());
            attackCooldownTimer = 1.8f;
        }
    }

    private IEnumerator SlapAttackRoutine()
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
                    Quaternion baseRot = Quaternion.LookRotation(lookDir) * Quaternion.Euler(0f, -90f, 0f);
                    transform.rotation = Quaternion.Slerp(transform.rotation, baseRot, 14f * Time.deltaTime);
                }
            }

            yield return null;
        }

        TriggerAttackAnimation();

        Vector3 currentEuler = transform.eulerAngles;
        Sequence slapSeq = DOTween.Sequence();

        slapSeq.Append(transform.DORotate(new Vector3(currentEuler.x, currentEuler.y - 35f, currentEuler.z), 0.1f).SetEase(Ease.OutQuad));
        slapSeq.Append(transform.DORotate(new Vector3(currentEuler.x, currentEuler.y + 60f, currentEuler.z), slapDuration).SetEase(Ease.InQuad));

        yield return new WaitForSeconds(0.1f + slapDuration * 0.4f);
        CheckHitbox();

        yield return slapSeq.WaitForCompletion();

        transform.DORotate(currentEuler, 0.15f);
        yield return new WaitForSeconds(0.15f);

        isAttacking = false;
    }

    private void CheckHitbox()
    {
        Vector3 forwardDir = transform.right; 
        Vector3 hitboxCenter = transform.position + (forwardDir * hitboxForwardOffset);

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