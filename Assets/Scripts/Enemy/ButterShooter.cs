using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButterShooter : EnemyBase
{
    [SerializeField] private Animator animator;

    [Header("Shooting Stats")]
    public GameObject projectile;
    public float bulletSpeed = 15f;
    public float maxDistance = 30f;
    public float distanceTraveled = 0f;
    public float hitRadius = 0.8f;

    [Header("Movement Thresholds")]
    public float distanceBeforeStop = 4f; 
    public float distanceBeforeShoot = 6f;
    public float orbitSpeed = 45f;        

    public bool isAttacking = false;

    private Transform player;
    private float attackTimer;

    protected override void Start()
    {
        base.Start();
        player = FindFirstObjectByType<PlayerCharacter>().transform;
        attackTimer = attackCooldown;
    }

    private void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(player.position, transform.position);

        if (distance <= distanceBeforeStop)
        {
            isAttacking = true;
        }
        else if (distance > distanceBeforeShoot)
        {
            isAttacking = false;
        }


        transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));

        if (!isAttacking)
        {
            transform.position += transform.forward * (currentspeed * Time.deltaTime);
        }
        else
        {
            transform.RotateAround(player.position, Vector3.up, orbitSpeed * Time.deltaTime);

            if (distance > distanceBeforeStop)
            {
                transform.position += transform.forward * (currentspeed * Time.deltaTime);
            }
            else if (distance < distanceBeforeStop - 0.5f)
            {
                transform.position -= transform.forward * (currentspeed * Time.deltaTime);
            }

            attackTimer -= Time.deltaTime;
            if (attackTimer <= 0)
            {
                PerformShoot();
                attackTimer = attackCooldown;
            }
        }
    }

    private void PerformShoot()
    {
        if (animator != null)
        {
        }

        if (projectile != null)
        {
            Instantiate(projectile, transform.position, transform.rotation);
        }
    }
}