using UnityEngine;

public class ChargerEnemy : EnemyBase
{
    [Header("Charge Movement Settings")]
    [SerializeField] private float chargeSpeed = 16f;
    public float chargeDistance = 12f;
    public float maxChargeTime = 2.5f;

    [Header("Hitbox Detection")]
    [SerializeField] private float hitRadius = 1.0f;
    [SerializeField] private float forwardOffset = 0.8f;
    [SerializeField] private float heightOffset = 0.5f;
    [SerializeField] private LayerMask hitMask = ~0;

    [Header("Damage Settings")]
    [SerializeField] private float hitDamageCooldown = 0.5f;

    private bool isCharging = false;
    private float chargeTimer = 0f;
    private float lastDamageTime = -99f;
    private Rigidbody rb;

    private static readonly int IsAttackingHash = Animator.StringToHash("isAttacking");

    protected override void Start()
    {
        maxHP = 10f;
        damage = 20f;
        moveSpeed = 4.5f;
        preferRune = true;
        base.Start();

        rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        if (enemyAnimator != null)
        {
            enemyAnimator.enabled = true;
            enemyAnimator.applyRootMotion = false;
        }

        if (hitMask.value == 0) hitMask = ~0;
    }

    protected override void BehaviorUpdate()
    {
        if (currentTarget == null) return;

        Vector3 toTarget = currentTarget.position - transform.position;
        toTarget.y = 0f;
        Vector3 moveDir = toTarget.sqrMagnitude > 0.001f ? toTarget.normalized : -transform.right;

        if (isCharging)
        {
            chargeTimer += Time.deltaTime;

            transform.position += moveDir * chargeSpeed * Time.deltaTime;

            Quaternion targetRot = Quaternion.LookRotation(moveDir) * Quaternion.Euler(0f, 90f, 0f);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, 14f * Time.deltaTime);

            CheckHitbox();

            if (chargeTimer >= maxChargeTime)
            {
                StopCharge();
            }

            return;
        }

        transform.position += moveDir * moveSpeed * Time.deltaTime;

        Quaternion walkRot = Quaternion.LookRotation(moveDir) * Quaternion.Euler(0f, 90f, 0f);
        transform.rotation = Quaternion.Slerp(transform.rotation, walkRot, 10f * Time.deltaTime);

        if (enemyAnimator != null)
        {
            enemyAnimator.SetBool(IsMovingHash, true);
        }

        if (attackCooldownTimer <= 0f && Vector3.Distance(transform.position, currentTarget.position) < chargeDistance)
        {
            StartCharge();
        }
    }

    private void StartCharge()
    {
        isCharging = true;
        chargeTimer = 0f;
        attackCooldownTimer = 7f;

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
            enemyAnimator.SetBool(IsMovingHash, true);
        }
    }

    private Vector3 GetHitCenter()
    {
        return transform.position + Vector3.up * heightOffset + (-transform.right * forwardOffset);
    }

    private void CheckHitbox()
    {
        if (Time.time < lastDamageTime + hitDamageCooldown) return;

        Collider[] hits = Physics.OverlapSphere(GetHitCenter(), hitRadius, hitMask, QueryTriggerInteraction.Collide);

        for (int i = 0; i < hits.Length; i++)
        {
            if (TryDealDamage(hits[i])) break;
        }
    }

    private bool TryDealDamage(Collider col)
    {
        if (col == null || col.transform == transform || col.transform.IsChildOf(transform)) return false;

        PlayerHealthAndStat health = col.GetComponentInParent<PlayerHealthAndStat>();
        if (health == null) health = col.GetComponentInChildren<PlayerHealthAndStat>();
        if (health == null) health = col.GetComponent<PlayerHealthAndStat>();

        if (health != null)
        {
            health.takeDamage(damage);
            lastDamageTime = Time.time;
            if (CameraShake.Instance != null) CameraShake.Instance.ShakeLight();
            return true;
        }

        RuneManager rune = col.GetComponentInParent<RuneManager>();
        if (rune == null) rune = col.GetComponentInChildren<RuneManager>();
        if (rune == null) rune = col.GetComponent<RuneManager>();

        if (rune != null)
        {
            rune.currentHealth = Mathf.Max(0f, rune.currentHealth - damage);
            lastDamageTime = Time.time;
            return true;
        }

        return false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isCharging) TryDealDamage(collision.collider);
    }

    private void OnCollisionStay(Collision collision)
    {
        if (isCharging && Time.time >= lastDamageTime + hitDamageCooldown)
        {
            TryDealDamage(collision.collider);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isCharging) TryDealDamage(other);
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
        Gizmos.DrawWireSphere(GetHitCenter(), hitRadius);
    }
}