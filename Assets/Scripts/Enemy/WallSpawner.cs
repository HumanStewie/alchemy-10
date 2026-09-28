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

        smallWallPrefab = (GameObject )Resources.Load("Prefab/Small Wall");
    }

    protected override void BehaviorUpdate()
    {
        if (currentTarget != null)
            MoveTowards(currentTarget.position, currentspeed * 0.6f);

        if (attackCooldownTimer <= 0f)
        {
            SpawnWalls();
            attackCooldownTimer = 7f;
        }
    }

    void SpawnWalls()
    {
        if (smallWallPrefab == null || currentTarget == null) return;

        Vector3 forward = (currentTarget.position - transform.position).normalized;
        Vector3 pos1 = currentTarget.position + forward * spawnDistance;
        Vector3 pos2 = currentTarget.position - forward * spawnDistance;

        Instantiate(smallWallPrefab, pos1, Quaternion.LookRotation(forward));
        Instantiate(smallWallPrefab, pos2, Quaternion.LookRotation(-forward));

        if (MusicManager.Instance != null)
            MusicManager.Instance.PlayWallSpawnerSound(transform.position);
    }
}