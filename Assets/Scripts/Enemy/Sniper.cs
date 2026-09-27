using System.Collections;
using UnityEngine;

public class SniperEnemy : EnemyBase
{
    [Header("Sniper Settings")]
    public LineRenderer aimLine;
    public float stoppingDistance = 15f;
    public LayerMask obstacleMask;

    private Transform player;
    private float attackTimer;
    private bool isAiming = false;

    public GameObject bullet;

    protected override void Start()
    {
        base.Start();
        var pc = FindAnyObjectByType<PlayerCharacter>();
        if (pc != null) player = pc.transform;

        attackTimer = attackCooldown;

        if (aimLine != null) aimLine.enabled = false;
    }

    void Update()
    {
        if (player == null || isAiming) return;

        float dist = Vector3.Distance(transform.position, player.position);
        bool canSeePlayer = HasLineOfSight();

        if (!canSeePlayer || dist > stoppingDistance)
        {
            transform.position = Vector3.MoveTowards(transform.position, player.position, currentspeed * Time.deltaTime);
            transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));
        }
        else
        {
            transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));

            if (attackTimer > 0)
            {
                attackTimer -= Time.deltaTime;
            }
            else
            {
                StartCoroutine(AimAndShoot());
            }
        }
    }

    private bool HasLineOfSight()
    {
        if (player == null) return false;

        Vector3 origin = transform.position + Vector3.up * 1f;
        Vector3 target = player.position + Vector3.up * 1f;
        Vector3 direction = target - origin;
        float maxDist = Vector3.Distance(origin, target);

        if (Physics.Raycast(origin, direction, out RaycastHit hit, maxDist, obstacleMask))
        {
            if (hit.transform != player && !hit.transform.IsChildOf(player))
            {
                return false;
            }
        }

        return true;
    }

    IEnumerator AimAndShoot()
    {
        isAiming = true;

        float aimDuration = 1f;
        float elapsed = 0f;

        while (elapsed < aimDuration)
        {
            elapsed += Time.deltaTime;

            if (aimLine != null && player != null)
            {
                aimLine.SetPosition(0, transform.position + Vector3.up * 1f);
                aimLine.SetPosition(1, player.position + Vector3.up * 1f);
                aimLine.enabled = (Mathf.FloorToInt(elapsed * 10f) % 2 == 0);
            }

            yield return null;
        }

        if (aimLine != null) aimLine.enabled = true;
        yield return new WaitForSeconds(0.15f);

        if (bullet != null && player != null)
        {
            Vector3 spawnPos = transform.position + Vector3.up * 1f;
            Quaternion rot = Quaternion.LookRotation((player.position - spawnPos).normalized);
            Instantiate(bullet, spawnPos, rot);
        }

        if (aimLine != null) aimLine.enabled = false;
        attackTimer = attackCooldown;
        isAiming = false;
    }
}