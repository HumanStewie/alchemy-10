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

        Vector3 lookDir = currentTarget.position - transform.position;
        lookDir.y = 0f;

        if (lookDir.sqrMagnitude > 0.001f)
        {
            transform.rotation = Quaternion.LookRotation(lookDir) * Quaternion.Euler(0f, 180f, 0f);
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (BookMovement.Instance != null)
            {
                BookMovement.Instance.Disabler(4);
            }

            if (MusicManager.Instance != null)
            {
                MusicManager.Instance.PlayJamToucherSound(transform.position);
            }

            TriggerAttackAnimation();
        }
    }
}