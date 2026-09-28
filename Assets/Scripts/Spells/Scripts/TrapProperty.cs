using System.Collections.Generic;
using UnityEngine;

public class TrapProperty : MonoBehaviour
{
    [Header("Trap Settings")]
    public float lifetime = 10f;
    public float damage = 4f;
    public float damageInterval = 1f;
    public GameObject explosionVFX;

    [Header("Explosion Upgrade Settings")]
    public float deathExplosionRadius = 3f;
    public float deathExplosionDamage = 10f;

    private Dictionary<Collider, float> activeEnemies = new Dictionary<Collider, float>();
    private List<Collider> toRemove = new List<Collider>();
    private bool isQuitting = false;

    private void Start()
    {
        ApplyUpgradesFromLiveTemplate();
        Destroy(gameObject, lifetime);
    }

    private BreadTrap GetLiveTrap()
    {
        if (GestureRecognizer.Instance == null || GestureRecognizer.Instance.templates == null)
            return null;

        return GestureRecognizer.Instance.templates.Find(s => s is BreadTrap) as BreadTrap;
    }

    private void ApplyUpgradesFromLiveTemplate()
    {
        BreadTrap trap = GetLiveTrap();
        if (trap == null) return;

        if (trap.isTier12)
        {
            lifetime = 15f;
            transform.localScale *= 1.5f;
        }
    }

    private void Update()
    {
        toRemove.Clear();
        List<Collider> keys = new List<Collider>(activeEnemies.Keys);

        foreach (Collider enemy in keys)
        {
            if (enemy == null)
            {
                toRemove.Add(enemy);
                continue;
            }

            if (Time.time >= activeEnemies[enemy] + damageInterval)
            {
                ApplyEffects(enemy);
                activeEnemies[enemy] = Time.time;
            }
        }

        foreach (Collider col in toRemove)
            activeEnemies.Remove(col);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy") && !activeEnemies.ContainsKey(other))
        {
            ApplyEffects(other);
            activeEnemies.Add(other, Time.time);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy") && activeEnemies.ContainsKey(other))
            activeEnemies.Remove(other);
    }

    private void ApplyEffects(Collider enemy)
    {
        EnemyBase enemyScript = enemy.GetComponentInParent<EnemyBase>();
        if (enemyScript == null) return;

        enemyScript.takeDamage(damage);

        if (explosionVFX != null)
            Instantiate(explosionVFX, enemy.transform.position, Quaternion.identity);

        BreadTrap trap = GetLiveTrap();
        if (trap == null) return;

        if (trap.isTier11)
            enemyScript.Poisoned(1f, 5f);

        if (trap.isTier21)
            enemyScript.Burnt(0.5f, 6f);
    }

    private void OnDestroy()
    {
        if (isQuitting || !gameObject.scene.isLoaded) return;

        BreadTrap trap = GetLiveTrap();
        if (trap != null && trap.isTier22)
        {
            if (explosionVFX != null)
                Instantiate(explosionVFX, transform.position, Quaternion.identity);

            Collider[] hits = Physics.OverlapSphere(transform.position, deathExplosionRadius);
            foreach (Collider col in hits)
            {
                if (col.CompareTag("Enemy"))
                {
                    EnemyBase enemy = col.GetComponentInParent<EnemyBase>();
                    if (enemy != null)
                        enemy.takeDamage(deathExplosionDamage);
                }
            }
        }
    }

    private void OnApplicationQuit()
    {
        isQuitting = true;
    }
}