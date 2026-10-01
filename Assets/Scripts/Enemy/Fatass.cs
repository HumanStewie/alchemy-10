using UnityEngine;

public class Fatass : EnemyBase
{
    protected override void Start()
    {
        maxHP = 25f;
        damage = 0f;
        moveSpeed = 3f;
        preferRune = false;
        base.Start();
    }

    protected override void BehaviorUpdate()
    {
        MoveTowards(currentTarget != null ? currentTarget.position : transform.position, currentspeed * 0.7f);

        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(currentTarget.position - transform.position) * Quaternion.Euler(0f, 110f, 0f), 14f * Time.deltaTime);

        if (attackCooldownTimer <= 0f)
        {
            Collider[] hits = Physics.OverlapSphere(transform.position, 2.5f);
            foreach (var h in hits)
            {
                if (h.GetComponent<JamSpread>() || h.GetComponent<TrapProperty>() || h.GetComponent<JamProperty>() || h.GetComponent<WallProp>())
                {
                    TriggerAttackAnimation();

                    Destroy(h.gameObject);
                    attackCooldownTimer = 10f;
                    break;
                }
            }
        }
    }
}