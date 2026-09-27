using UnityEngine;

public class KnockedEnemy : MonoBehaviour
{
    private Vector3 knockVelocity;
    private float collisionBonusDamage;
    private bool isKnocked = false;
    private float timer = 0f;
    private float maxDuration = 0.6f;
    private EnemyBase enemyScript;

    public void Launch(Vector3 velocity, float collisionDamage)
    {
        knockVelocity = velocity;
        collisionBonusDamage = collisionDamage;
        isKnocked = true;
        timer = 0f;
        enemyScript = GetComponent<EnemyBase>();
    }

    void Update()
    {
        if (!isKnocked) return;

        timer += Time.deltaTime;
        if (timer >= maxDuration)
        {
            EndKnockback();
            return;
        }

        float step = knockVelocity.magnitude * Time.deltaTime;
        Ray ray = new Ray(transform.position, knockVelocity.normalized);

        // Check if the enemy collides with obstacles or other enemies while flying
        if (Physics.Raycast(ray, out RaycastHit hit, step))
        {
            if (hit.collider.gameObject == gameObject) return;

            // Hit a wall or obstacle
            if (!hit.collider.CompareTag("Player"))
            {
                TriggerCollisionDamage(hit);
                return;
            }
        }

        transform.position += knockVelocity * Time.deltaTime;
        knockVelocity = Vector3.Lerp(knockVelocity, Vector3.zero, Time.deltaTime * 6f);
    }

    private void TriggerCollisionDamage(RaycastHit hit)
    {
        // 1. Damage self on impact
        if (enemyScript != null)
        {
            enemyScript.takeDamage(collisionBonusDamage);
        }

        EnemyBase otherEnemy = hit.collider.GetComponentInParent<EnemyBase>();
        if (otherEnemy != null && otherEnemy != enemyScript)
        {
            otherEnemy.takeDamage(collisionBonusDamage);
        }

        EndKnockback();
    }

    private void EndKnockback()
    {
        isKnocked = false;
        Destroy(this);
    }
}