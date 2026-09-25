using System.Collections;
using UnityEngine;

public class SimpleFollow : EnemyBase
{
    [SerializeField] private float attackRange = 1.5f;
    private Transform player;
    [SerializeField] private float attackTimer;
    

    [Header("Hitbox Settings")]
    public Vector3 hitboxOffset = new Vector3(0, 0, 1f); 
    public float hitboxRadius = 1f; 
    public LayerMask playerLayer;

    [SerializeField] private float preWarmAttack = 0.5f;
    protected override void Start()
    {
        base.Start();
        player = FindAnyObjectByType<PlayerCharacter>().transform;
        attackTimer = attackCooldown;
    }

    void Update()
    {
        if (Vector3.Distance(transform.position, player.position) > attackRange)
        {
            transform.position = Vector3.MoveTowards(transform.position, player.position, currentspeed * Time.deltaTime);
        }
        else
        {
            if (attackTimer <=0)
            {
                StartCoroutine(AttackSeq());
            }
        }
    }

    IEnumerator AttackSeq()
    {
        Vector3 hitCenter = transform.position + transform.TransformDirection(hitboxOffset);
        attackTimer = attackCooldown;

        yield return new WaitForSeconds(preWarmAttack);


        Collider[] hitTargets = Physics.OverlapSphere(hitCenter, hitboxRadius, playerLayer);

        foreach (Collider hit in hitTargets)
        {
            if (hit.CompareTag("Player"))
            {
                hit.GetComponent<PlayerHealthAndStat>().takeDamage(5);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector3 hitCenter = transform.position + transform.TransformDirection(hitboxOffset);
        Gizmos.DrawWireSphere(hitCenter, hitboxRadius);
    }
}
