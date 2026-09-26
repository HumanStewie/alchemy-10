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

        player = FindAnyObjectByType<PlayerCharacter>().transform;

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

        if (dist > 7)
        {
            transform.position = Vector3.MoveTowards(transform.position, player.position, currentspeed * Time.deltaTime);
        }
        else
        {
            if (attackTimer <= 0 && !isSummoning)
            {
                StartCoroutine(SummonWall());
            }
        }
    }

    IEnumerator SummonWall()
    {
        isSummoning = true;

        yield return new WaitForSeconds(2f);

        if (bookMovement != null && player != null)
        {
            GameObject spawnedWall = Instantiate(wall, bookMovement.WallSpawnLoc.position, Quaternion.Euler(-90, player.eulerAngles.y - 90f, 0));
            spawnedWall.transform.DOMoveY(2.75f, 0.5f).SetEase(Ease.InOutSine);
        }
        attackTimer = attackCooldown;
        isSummoning = false;
    }
}