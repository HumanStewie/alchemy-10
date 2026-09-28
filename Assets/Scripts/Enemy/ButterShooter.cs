using UnityEngine;

public class ShootingEnemy : EnemyBase
{
    [Header("Shooting")]
    public GameObject butterProjectile;
    public float preferredDistance = 4f;
    public float circleSpeed = 3f;

    private bool isCircling = false;

    protected override void Start()
    {
        maxHP = 7f;
        damage = 3f;
        moveSpeed = 4f;
        preferRune = true;
        base.Start();
    }

    protected override void BehaviorUpdate()
    {
        if (currentTarget == null) return;

        float dist = Vector3.Distance(transform.position, currentTarget.position);

        if (dist > preferredDistance + 1f)
        {
            isCircling = false;
            MoveTowards(currentTarget.position, currentspeed);
        }
        else
        {
            isCircling = true;
            Vector3 dir = (transform.position - currentTarget.position).normalized;
            Vector3 tangent = Vector3.Cross(Vector3.up, dir);
            transform.position += tangent * circleSpeed * Time.deltaTime;
            transform.LookAt(new Vector3(currentTarget.position.x, transform.position.y, currentTarget.position.z));
        }

        if (attackCooldownTimer <= 0f)
        {
            Shoot();
            attackCooldownTimer = 5f;
        }
    }

    void Shoot()
    {
        if (butterProjectile == null || currentTarget == null) return;

        Vector3 spawnPos = transform.position + transform.forward * 1.2f + Vector3.up * 0.5f;
        GameObject proj = Instantiate(butterProjectile, spawnPos, Quaternion.identity);

        Vector3 dir = (currentTarget.position - spawnPos).normalized;
        if (proj.TryGetComponent<Rigidbody>(out var rb))
            rb.linearVelocity = dir * 18f;

        if (MusicManager.Instance != null)
            MusicManager.Instance.PlayButterShooterSound(transform.position);
    }
}