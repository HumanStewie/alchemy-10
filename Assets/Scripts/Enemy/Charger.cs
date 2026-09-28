using UnityEngine;

public class ChargerEnemy : EnemyBase
{
    public float chargeSpeed = 18f;
    public float chargeDistance = 12f;
    private bool isCharging = false;
    private Vector3 chargeDir;

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
            transform.position += chargeDir * chargeSpeed * Time.deltaTime;

            if (Vector3.Distance(transform.position, currentTarget.position) < 1.5f || attackCooldownTimer < 8f)
            {
                isCharging = false;
                if (currentTarget.CompareTag("Player"))
                {
                    var health = currentTarget.GetComponent<PlayerHealthAndStat>();
                    if (health != null) health.takeDamage(damage);
                }
            }
            return;
        }

        MoveTowards(currentTarget.position, currentspeed);

        if (attackCooldownTimer <= 0f && Vector3.Distance(transform.position, currentTarget.position) < chargeDistance)
        {
            isCharging = true;
            chargeDir = (currentTarget.position - transform.position).normalized;
            chargeDir.y = 0;
            attackCooldownTimer = 10f;

            if (MusicManager.Instance != null)
                MusicManager.Instance.PlayChargerSound(transform.position);
        }
    }
}