using System.Collections;
using UnityEngine;

public class SimpleFollow : EnemyBase
{
    [SerializeField] private float attackRange = 1.5f;
    private Transform player;
    private float attackTimer;

    [Header("Hitbox Settings")]
    public Vector3 hitboxOffset = new Vector3(0, 0, 1f);
    public float hitboxRadius = 1f;
    public LayerMask playerLayer;

    [SerializeField] private float preWarmAttack = 0.5f;
    private bool isAttacking;

    protected override void Start()
    {
        base.Start();
        var pc = FindAnyObjectByType<PlayerCharacter>();
        if (pc != null) player = pc.transform;
        attackTimer = attackCooldown;
    }

    void Update()
    {
        if (player == null || isAttacking) return;

        attackTimer -= Time.deltaTime;

        if (Vector3.Distance(transform.position, player.position) > attackRange)
        {
            transform.position = Vector3.MoveTowards(transform.position, player.position, currentspeed * Time.deltaTime);
            transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));
        }
        else
        {
            if (attackTimer <= 0f)
            {
                StartCoroutine(AttackSeq());
            }
        }
    }

    IEnumerator AttackSeq()
    {
        isAttacking = true;
        attackTimer = attackCooldown;

        yield return new WaitForSeconds(preWarmAttack);

        if (player == null)
        {
            isAttacking = false;
            yield break;
        }

        Vector3 hitCenter = transform.position + transform.TransformDirection(hitboxOffset);
        Collider[] hitTargets = Physics.OverlapSphere(hitCenter, hitboxRadius, playerLayer);

        foreach (Collider hit in hitTargets)
        {
            if (hit.CompareTag("Player"))
            {
                var health = hit.GetComponent<PlayerHealthAndStat>();
                if (health != null)
                {
                    health.takeDamage(currentdamage > 0 ? currentdamage : 5f);
                }
            }
        }

        isAttacking = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector3 hitCenter = transform.position + transform.TransformDirection(hitboxOffset);
        Gizmos.DrawWireSphere(hitCenter, hitboxRadius);
    }
}