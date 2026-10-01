using UnityEngine;

public class ChargerEnemy : EnemyBase
{
    [Header("Charge Settings")]
    public float chargeSpeed = 18f;
    public float chargeDistance = 12f;
    public float maxChargeTime = 2.5f;

    [Header("Impact Hitbox")]
    [SerializeField] private float impactRadius = 1.0f;
    [SerializeField] private float impactForwardOffset = 1.0f;
    [SerializeField] private LayerMask obstacleMask;

    private bool isCharging = false;
    private float chargeTimer = 0f;
    private Vector3 chargeDir;

    private static readonly int IsAttackingHash = Animator.StringToHash("isAttacking");

    protected override void Start()
    {
        maxHP = 10f;
        damage = 20f;
        moveSpeed = 4f;
        preferRune = true;
        base.Start();
    }

    protected override void BehaviorUpdate()
    {
        if (isCharging)
        {
            chargeTimer += Time.deltaTime;

            // Roll forward
            transform.position += chargeDir * chargeSpeed * Time.deltaTime;

            // Keep facing rolling direction with the +90 deg visual offset
            Quaternion targetRot = Quaternion.LookRotation(chargeDir) * Quaternion.Euler(0f, 90f, 0f);
            transform.rotation = targetRot;

            // Check for impacts
            if (CheckImpact() || chargeTimer >= maxChargeTime)
            {
                StopCharge();
            }

            return;
        }

        if (currentTarget == null) return;

        MoveTowards(currentTarget.position, currentspeed);

        if (attackCooldownTimer <= 0f && Vector3.Distance(transform.position, currentTarget.position) < chargeDistance)
        {
            StartCharge();
        }
    }

    private void StartCharge()
    {
        isCharging = true;
        chargeTimer = 0f;
        attackCooldownTimer = 8f;

        chargeDir = (currentTarget.position - transform.position);
        chargeDir.y = 0f;
        chargeDir.Normalize();

        if (chargeDir.sqrMagnitude < 0.01f)
            chargeDir = transform.right; 

        if (enemyAnimator != null)
        {
            enemyAnimator.SetBool(IsMovingHash, false);
            enemyAnimator.SetBool(IsAttackingHash, true);
        }

        if (MusicManager.Instance != null)
            MusicManager.Instance.PlayChargerSound(transform.position);

        if (CameraShake.Instance != null)
            CameraShake.Instance.ShakeHeavy();
    }

    private void StopCharge()
    {
        isCharging = false;
        if (enemyAnimator != null)
        {
            enemyAnimator.SetBool(IsAttackingHash, false);
        }
    }

    private bool CheckImpact()
    {
        Vector3 forwardDir = transform.right;
        Vector3 checkPos = transform.position + Vector3.up * 0.5f + (forwardDir * impactForwardOffset);

        Collider[] hits = Physics.OverlapSphere(checkPos, impactRadius);

        for (int i = 0; i < hits.Length; i++)
        {
            Collider col = hits[i];
            if (col.transform == transform || col.transform.IsChildOf(transform)) continue;

            if (col.CompareTag("Player") || col.TryGetComponent<PlayerHealthAndStat>(out var _))
            {
                var health = col.GetComponentInParent<PlayerHealthAndStat>();
                if (health != null) health.takeDamage(damage);
                return true;
            }

            if (col.TryGetComponent<RuneManager>(out var rune))
            {
                rune.currentHealth = Mathf.Max(0f, rune.currentHealth - damage);
                return true;
            }

            if (((1 << col.gameObject.layer) & obstacleMask) != 0 && !col.isTrigger)
            {
                return true;
            }
        }

        return false;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        isCharging = false;
        if (enemyAnimator != null)
        {
            enemyAnimator.SetBool(IsAttackingHash, false);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector3 forwardDir = Application.isPlaying ? transform.right : transform.forward;
        Vector3 checkPos = transform.position + Vector3.up * 0.5f + (forwardDir * impactForwardOffset);
        Gizmos.DrawWireSphere(checkPos, impactRadius);
    }
}