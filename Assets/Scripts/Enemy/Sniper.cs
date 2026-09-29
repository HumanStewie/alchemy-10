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
            MusicManager.Instance.PlayLazerChargeSound(transform.position);
        }

        if (isAiming)
        {
            aimTimer -= Time.deltaTime;
            UpdateAimLine();

            if (aimTimer <= 0f)
            {
                Shoot();
                MusicManager.Instance.PlaySniperSound(transform.position);

                isAiming = false;
                if (aimLine != null) aimLine.enabled = false;
                attackCooldownTimer = 5f;
            }
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