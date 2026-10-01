using UnityEngine;

public class WallSpawnerEnemy : EnemyBase
{
    public GameObject smallWallPrefab;
    public float spawnDistance = 3f;

    protected override void Start()
    {
        maxHP = 15f;
        damage = 0f;
        moveSpeed = 3.5f;
        preferRune = true;
        base.Start();

        smallWallPrefab = Resources.Load<GameObject>("Prefab/Small Wall");
    }

    protected override void BehaviorUpdate()
    {
        if (currentTarget != null)
        {
            MoveTowards(currentTarget.position, currentspeed * 0.6f);

            Vector3 lookDir = currentTarget.position - transform.position;
            lookDir.y = 0f;
            if (lookDir.sqrMagnitude > 0.01f)
            {
                transform.rotation = Quaternion.LookRotation(lookDir);
            }
        }

        if (attackCooldownTimer <= 0f)
        {
            SpawnWalls();
            attackCooldownTimer = 7f;
        }
    }

    void SpawnWalls()
    {
        if (smallWallPrefab == null || currentTarget == null) return;

        Vector3 forward = (currentTarget.position - transform.position);
        forward.y = 0f;
        forward.Normalize();

        Vector3 pos1 = currentTarget.position + forward * spawnDistance + Vector3.up;
        Vector3 pos2 = currentTarget.position - forward * spawnDistance+ Vector3.up;

        TriggerAttackAnimation();

        Quaternion wallRot1 = Quaternion.LookRotation(forward) * Quaternion.Euler(-90f, 90f, 0f);
        Quaternion wallRot2 = Quaternion.LookRotation(-forward) * Quaternion.Euler(-90f, 90f, 0f);

        Instantiate(smallWallPrefab, pos1, wallRot1);
        Instantiate(smallWallPrefab, pos2, wallRot2);

        if (MusicManager.Instance != null)
            MusicManager.Instance.PlayWallSpawnerSound(transform.position);
    }
}