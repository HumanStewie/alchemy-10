using UnityEngine;

public class JamToucherEnemy : EnemyBase
{
    public float jamBlockDuration = 4f;

    protected override void Start()
    {
        maxHP = 1f;
        damage = 0f;
        moveSpeed = 8f;
        preferRune = false;
        base.Start();
    }

    protected override void BehaviorUpdate()
    {
        if (currentTarget == null) return;
        MoveTowards(currentTarget.position, currentspeed);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            BookMovement.Instance.TemporaryDisable(4);
            if (MusicManager.Instance != null)

                MusicManager.Instance.PlayJamToucherSound(transform.position);
            TriggerAttackAnimation();
        }
    }
}