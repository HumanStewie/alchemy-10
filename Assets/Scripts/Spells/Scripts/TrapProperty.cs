using System.Collections.Generic;
using UnityEngine;

public class TrapProperty : MonoBehaviour
{
    public static List<TrapProperty> ActiveTraps = new List<TrapProperty>();
    private const int MaxTrapCount = 2;

    [Header("Trap Settings")]
    public float lifetime = 10f;
    public float damage = 4f;
    [Tooltip("Tick damage delay")]
    public float damageInterval = 2f;
    public GameObject explosionVFX;

    [Header("Explosion Upgrade Settings")]
    public float deathExplosionRadius = 3.5f;
    public float deathExplosionDamage = 10f;

    private Dictionary<EnemyBase, float> activeEnemies = new Dictionary<EnemyBase, float>();
    private List<EnemyBase> toRemove = new List<EnemyBase>();
    private bool isQuitting = false;

    private void Awake()
    {
        ActiveTraps.Add(this);
        if (ActiveTraps.Count > MaxTrapCount)
        {
            TrapProperty oldestTrap = ActiveTraps[0];
            ActiveTraps.RemoveAt(0);
            if (oldestTrap != null)
            {
                Destroy(oldestTrap.gameObject);
            }
        }
    }

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
        List<EnemyBase> keys = new List<EnemyBase>(activeEnemies.Keys);

        foreach (EnemyBase enemy in keys)
        {
            if (enemy == null || !enemy.gameObject.activeInHierarchy)
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

        foreach (EnemyBase deadEnemy in toRemove)
        {
            activeEnemies.Remove(deadEnemy);
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player")) return;

        EnemyBase enemy = other.gameObject.GetComponentInParent<EnemyBase>();
        if (enemy == null)
        {
            enemy = other.gameObject.GetComponentInChildren<EnemyBase>();
        }

        if (enemy != null && !activeEnemies.ContainsKey(enemy))
        {
            ApplyEffects(enemy);
            activeEnemies.Add(enemy, Time.time);
        }
    }

    private void OnCollisionExit(Collision other)
    {
        EnemyBase enemy = other.gameObject.GetComponentInParent<EnemyBase>();
        if (enemy == null)
        {
            enemy = other.gameObject.GetComponentInChildren<EnemyBase>();
        }

        if (enemy != null && activeEnemies.ContainsKey(enemy))
        {
            activeEnemies.Remove(enemy);
        }
    }

    private void ApplyEffects(EnemyBase enemyScript)
    {
        if (enemyScript == null) return;

        enemyScript.takeDamage(damage);

        if (explosionVFX != null)
        {
            Instantiate(explosionVFX, enemyScript.transform.position, Quaternion.identity);
        }

        BreadTrap trap = GetLiveTrap();
        if (trap == null) return;

        if (trap.isTier11)
        {
            enemyScript.Poisoned(1f, 5f);
        }

        if (trap.isTier21)
        {
            enemyScript.Burnt(0.5f, 6f);
        }
    }

    private void OnDestroy()
    {
        ActiveTraps.Remove(this);

        if (isQuitting || !gameObject.scene.isLoaded) return;

        BreadTrap trap = GetLiveTrap();
        if (trap != null && trap.isTier22)
        {
            if (explosionVFX != null)
            {
                Instantiate(explosionVFX, transform.position, Quaternion.identity);
            }

            if (CameraShake.Instance != null)
            {
                CameraShake.Instance.ShakeLight();
            }

            if (MusicManager.Instance != null)
            {
                MusicManager.Instance.PlayExplosionSound(transform.position);
            }

            Collider[] hits = Physics.OverlapSphere(transform.position, deathExplosionRadius);
            HashSet<EnemyBase> hitEnemies = new HashSet<EnemyBase>();

            foreach (Collider col in hits)
            {
                if (col.CompareTag("Player")) continue;

                EnemyBase enemy = col.GetComponentInParent<EnemyBase>();
                if (enemy != null && !hitEnemies.Contains(enemy))
                {
                    hitEnemies.Add(enemy);
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