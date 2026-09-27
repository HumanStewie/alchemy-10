using UnityEngine;

public class JamProperty : MonoBehaviour
{
    public enum JamType { Slippery, Sticky }
    public JamType patchType;

    [Header("Modifiers")]
    public float stickySpeedMultiplier = 0.4f;
    public float slipperySpeedMultiplier = 1.8f; 

    private void OnTriggerEnter(Collider other)
    {
        // 1. Affect Enemies
        EnemyBase enemy = other.GetComponent<EnemyBase>();
        if (enemy != null)
        {
            if (patchType == JamType.Sticky)
                enemy.currentspeed *= stickySpeedMultiplier;
            else if (patchType == JamType.Slippery)
                enemy.currentspeed *= slipperySpeedMultiplier;
        }

        if (patchType == JamType.Slippery && other.CompareTag("Projectile"))
        {
            Rigidbody rb = other.GetComponent<Rigidbody>();
            if (rb != null)
            {
                float currentSpeed = rb.linearVelocity.magnitude;
                rb.linearVelocity = transform.forward * currentSpeed;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Revert the speed changes when they leave the puddle
        EnemyBase enemy = other.GetComponent<EnemyBase>();
        if (enemy != null)
        {
            if (patchType == JamType.Sticky)
                enemy.currentspeed /= stickySpeedMultiplier;
            else if (patchType == JamType.Slippery)
                enemy.currentspeed /= slipperySpeedMultiplier;
        }
    }
}