using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 15f;
    public float hitRadius = 0.8f;
    public int damage = 5;


    public LayerMask layermask;

    private Transform player;

    private void Update()
    {
        transform.Translate(speed * Time.deltaTime * Vector3.forward, Space.Self);

    }
    private void Start()
    {
        player = FindFirstObjectByType<PlayerCharacter>().transform;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out EnemyBase enemy))
        {
            enemy.takeDamage(damage);
            Destroy(gameObject);
        }
    }
}