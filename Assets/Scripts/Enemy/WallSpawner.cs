using DG.Tweening;
using System.Collections;
using UnityEngine;

public class WallSpawner : EnemyBase
{
    public GameObject wall;

    private Transform player;
    private BookMovement bookMovement;
    private float attackTimer;
    private bool isSummoning;

    protected override void Start()
    {
        currentHP = maxHP;
        currentdamage = damage;
        currentspeed = speed;

        var pc = FindAnyObjectByType<PlayerCharacter>();
        if (pc != null) player = pc.transform;

        bookMovement = FindAnyObjectByType<BookMovement>();

        attackTimer = attackCooldown;
    }

    void Update()
    {
        if (player == null) return;

        if (attackTimer > 0)
        {
            attackTimer -= Time.deltaTime;
        }

        float dist = Vector3.Distance(player.position, transform.position);

        if (dist > 7f)
        {
            transform.position = Vector3.MoveTowards(transform.position, player.position, currentspeed * Time.deltaTime);
            transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));
        }
        else
        {
            if (attackTimer <= 0f && !isSummoning)
            {
                StartCoroutine(SummonWall());
            }
        }
    }

    IEnumerator SummonWall()
    {
        isSummoning = true;

        yield return new WaitForSeconds(2f);

        if (wall != null && bookMovement != null && bookMovement.WallSpawnLoc != null && player != null)
        {
            GameObject spawnedWall = Instantiate(
                wall,
                bookMovement.WallSpawnLoc.position,
                Quaternion.Euler(-90f, player.eulerAngles.y - 90f, 0f)
            );
            spawnedWall.transform.DOMoveY(2.75f, 0.5f).SetEase(Ease.InOutSine);
        }
        attackTimer = attackCooldown;
        isSummoning = false;
    }
}