using System.Collections;
using UnityEngine;

public class Charger : EnemyBase
{
    [Header("Charge Settings")]
    public float chargeSpeedMultiplier = 4f; 
    public float chargeDuration = 1f;       
    public float windUpTime = 0.5f;         
    public float hitboxRadius = 1.5f;       
    public LayerMask playerLayer;           

    private bool isAttacking;
    private float attackTimer;
    private Transform player;

    protected override void Start()
    {
        base.Start();
        attackTimer = attackCooldown;

        player = FindAnyObjectByType<PlayerCharacter>().transform;
    }

    void Update()
    {
        if (isAttacking) return;

        attackTimer -= Time.deltaTime;

        if (attackTimer <= 0)
        {
            StartCoroutine(AttackSeq());
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, player.position, currentspeed * Time.deltaTime);
            transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));
        }
    }

    IEnumerator AttackSeq()
    {
        isAttacking = true;

        transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));
        Vector3 chargeDirection = transform.forward;

        yield return new WaitForSeconds(windUpTime);

        float elapsed = 0f;
        bool hasHitPlayer = false;

        while (elapsed < chargeDuration)
        {
            elapsed += Time.deltaTime;

            transform.position += chargeDirection * (currentspeed * chargeSpeedMultiplier * Time.deltaTime);

            if (!hasHitPlayer)
            {
                Collider[] hits = Physics.OverlapSphere(transform.position, hitboxRadius, playerLayer);
                foreach (Collider hit in hits)
                {
                    if (hit.CompareTag("Player"))
                    {
                        hasHitPlayer = true;
                        break;
                    }
                }
            }

            yield return null; 
        }

        attackTimer = attackCooldown; 
        isAttacking = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, hitboxRadius);
    }
}