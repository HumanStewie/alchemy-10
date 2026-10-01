using UnityEngine;

public class SniperBullet : MonoBehaviour
{
    public float speed = 35f;
    public float lifetime = 5f;
    public float damage = 15f;

    private Vector3 moveDir;
    private GameObject shooter;
    private bool hasHit = false;

    public void Initialize(Vector3 direction, float dmg, GameObject sniperOwner)
    {
        moveDir = direction.normalized;
        damage = dmg;
        shooter = sniperOwner;
        transform.rotation = Quaternion.LookRotation(moveDir);

        if (shooter != null)
        {
            Collider bulletCol = GetComponent<Collider>();
            Collider[] shooterCols = shooter.GetComponentsInChildren<Collider>();
            if (bulletCol != null)
            {
                foreach (Collider col in shooterCols)
                {
                    Physics.IgnoreCollision(bulletCol, col, true);
                }
            }
        }

        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        if (hasHit) return;

        float step = speed * Time.deltaTime;
        Vector3 nextPos = transform.position + moveDir * step;

        if (Physics.Raycast(transform.position, moveDir, out RaycastHit hit, step))
        {
            CheckHit(hit.collider);
            transform.position = hit.point;
            Destroy(gameObject);
            hasHit = true;
            return;
        }

        transform.position = nextPos;
    }

    private void OnTriggerEnter(Collider other)
    {
        CheckHit(other);
    }

    private void OnCollisionEnter(Collision other)
    {
        CheckHit(other.collider);
        Destroy(gameObject);
    }

    private void CheckHit(Collider col)
    {
        if (col == null || hasHit) return;

        if (shooter != null && (col.gameObject == shooter || col.transform.IsChildOf(shooter.transform))) return;

        if (col.CompareTag("Player") || col.TryGetComponent<PlayerHealthAndStat>(out var _))
        {
            var health = col.GetComponentInParent<PlayerHealthAndStat>();
            if (health != null)
            {
                health.takeDamage(damage);
                hasHit = true;
                Destroy(gameObject);
                return;
            }
        }

        if (col.TryGetComponent<RuneManager>(out var rune))
        {
            rune.currentHealth = Mathf.Max(0f, rune.currentHealth - damage);
            hasHit = true;
            Destroy(gameObject);
            return;
        }
    }
}