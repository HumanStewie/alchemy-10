using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapProperty : MonoBehaviour
{
    [Header("Trap Settings")]
    public float lifetime = 10f;
    public float damage = 4f;
    public float damageInterval = 1f; // Ticks once every 1 second
    public GameObject explosionVFX;

    private Dictionary<Collider, float> activeEnemies = new();
    private List<Collider> toRemove = new List<Collider>();

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        toRemove.Clear();

        foreach (var entry in activeEnemies)
        {
            Collider enemy = entry.Key;

            if (enemy == null)
            {
                toRemove.Add(enemy);
                continue;
            }

            if (Time.time >= entry.Value + damageInterval)
            {
                ApplyDamage(enemy);
                toRemove.Add(enemy); 
            }
        }

        foreach (Collider col in toRemove)
        {
            if (col != null)
            {
                activeEnemies[col] = Time.time;
            }
            else
            {
                activeEnemies.Remove(col);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy") && !activeEnemies.ContainsKey(other))
        {
            ApplyDamage(other);
            activeEnemies.Add(other, Time.time);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy") && activeEnemies.ContainsKey(other))
        {
            activeEnemies.Remove(other);
        }
    }

    private void ApplyDamage(Collider enemy)
    {
        enemy.SendMessageUpwards("TakeDamage", damage, SendMessageOptions.DontRequireReceiver);

        if (explosionVFX != null)
        {
            Instantiate(explosionVFX, enemy.transform.position, Quaternion.identity);
        }
    }
}