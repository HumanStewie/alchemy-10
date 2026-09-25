using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButterShooter : EnemyBase
{
    [SerializeField] private Animator animator;


    public GameObject projectile;
    public float bulletSpeed = 15f;
    public float maxDistance = 30f;
    public float distanceTraveled = 0f;
    public float hitRadius = 0.8f;
    public int distanceBeforeShoot = 4;

    public bool isAttacking = false;
    void Start()
    {
    }
}