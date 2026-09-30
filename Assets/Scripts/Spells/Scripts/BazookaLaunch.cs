using UnityEngine;

public class BazookaLaunch : MonoBehaviour
{
    [Header("Impact Surface")]
    [SerializeField] private GameObject jamPatchPrefab;
    [SerializeField] private GameObject explosionVFX;
    [SerializeField] private float surfaceOffset = 0.02f;

    private float blastDamage = 15;
    private float blastRadius;
    private float freezeDuration;
    private Vector3 velocity;
    private bool hasHit = false;

    public void Launch(Vector3 direction, float speed, float damage, float radius, float freezeTime)
    {
        blastDamage = damage;
        blastRadius = radius;
        freezeDuration = freezeTime;

        velocity = direction.normalized * speed;

        Destroy(gameObject, 6f);
    }

    void Update()
    {
        if (hasHit) return;

        float moveDistance = velocity.magnitude * Time.deltaTime;

        if (Physics.Raycast(transform.position, velocity.normalized, out RaycastHit hit, moveDistance))
        {
            Explode(hit);
        }
        else
        {
            transform.position += velocity * Time.deltaTime;
            if (velocity != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(velocity);
            }
        }
    }

    private void Explode(RaycastHit hit)
    {
        hasHit = true;

        if (jamPatchPrefab != null)
        {
            Quaternion splatRotation = Quaternion.LookRotation(hit.normal);
            Vector3 splatPos = hit.point + (hit.normal * surfaceOffset);
            Instantiate(jamPatchPrefab, splatPos, splatRotation);
        }

        if (explosionVFX != null)
        {
            Instantiate(explosionVFX, hit.point, Quaternion.identity);
        }

        if (MusicManager.Instance != null)
        {
            MusicManager.Instance.PlayJamHitSound(hit.point);
        }

        Collider[] hits = Physics.OverlapSphere(hit.point, blastRadius);
        foreach (Collider col in hits)
        {
            if (col.CompareTag("Player")) continue;

            col.SendMessageUpwards("takeDamage", blastDamage, SendMessageOptions.DontRequireReceiver);

            if (freezeDuration > 0f)
            {
                col.SendMessageUpwards("Freeze", freezeDuration, SendMessageOptions.DontRequireReceiver);
            }
        }

        Destroy(gameObject);
    }
}