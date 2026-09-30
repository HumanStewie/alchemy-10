using UnityEngine;

public class SniperEnemy : EnemyBase
{
    public LineRenderer aimLine;
    public GameObject sniperBulletPrefab;
    public float aimDuration = 1f;

    private bool isAiming = false;
    private float aimTimer = 0f;

    protected override void Start()
    {
        maxHP = 15f;
        damage = 10f;
        moveSpeed = 2.5f;
        preferRune = false;
        base.Start();

        if (aimLine != null) aimLine.enabled = false;
    }

    protected override void BehaviorUpdate()
    {
        if (currentTarget == null) return;

        float dist = Vector3.Distance(transform.position, currentTarget.position);
        if (dist < 12f)
            MoveTowards(transform.position + (transform.position - currentTarget.position).normalized, currentspeed);

        if (!isAiming && attackCooldownTimer <= 0f)
        {
            StartAiming();
            if (MusicManager.Instance != null)
                MusicManager.Instance.PlayLazerChargeSound(transform.position);
        }

        if (isAiming)
        {
            Vector3 aimDir = (currentTarget.position - transform.position);
            aimDir.y = 0f;
            if (aimDir.sqrMagnitude > 0.01f)
            {
                Quaternion targetRot = Quaternion.LookRotation(aimDir) * Quaternion.Euler(0f, 0f, 0f);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, 10f * Time.deltaTime);
            }

            aimTimer -= Time.deltaTime;
            UpdateAimLine();

            if (aimTimer <= 0f)
            {
                Shoot();

                isAiming = false;
                if (aimLine != null) aimLine.enabled = false;
                attackCooldownTimer = 5f;
            }
        }
    }

    protected new void MoveTowards(Vector3 targetPos, float speed)
    {
        Vector3 dir = (targetPos - transform.position);
        dir.y = 0f;
        if (dir.sqrMagnitude > 0.01f)
        {
            transform.position += dir.normalized * speed * Time.deltaTime;
            Quaternion targetRot = Quaternion.LookRotation(dir) * Quaternion.Euler(0f, 180f, 0f);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, 8f * Time.deltaTime);
        }
    }

    void StartAiming()
    {
        isAiming = true;
        aimTimer = aimDuration;
        if (aimLine != null) aimLine.enabled = true;
    }

    void UpdateAimLine()
    {
        if (aimLine == null || currentTarget == null) return;
        aimLine.SetPosition(0, transform.position + Vector3.up * 1.5f);
        aimLine.SetPosition(1, currentTarget.position + Vector3.up * 1f);
    }

    void Shoot()
    {
        if (sniperBulletPrefab == null || currentTarget == null) return;

        Vector3 spawn = transform.position + Vector3.up * 1.5f;
        GameObject bullet = Instantiate(sniperBulletPrefab, spawn, Quaternion.identity);
        Vector3 dir = (currentTarget.position - spawn).normalized;

        if (bullet.TryGetComponent<Rigidbody>(out var rb))
            rb.linearVelocity = dir * 40f;

        if (MusicManager.Instance != null)
            MusicManager.Instance.PlaySniperSound(transform.position);
    }
}