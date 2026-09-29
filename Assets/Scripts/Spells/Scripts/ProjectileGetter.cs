using UnityEngine;

public class ProjectileGetter : MonoBehaviour
{
    public GameObject trapPrefab;
    [Header("Impact")]
    [SerializeField] private float surfaceOffset = 0.02f;
    [SerializeField] private LayerMask hitLayers = ~0;

    private Vector3 velocity;
    private bool hasHit = false;

    public void Launch(Vector3 direction, float speed)
    {
        velocity = direction.normalized * speed;
        Destroy(gameObject, 5f);
    }

    void Update()
    {
        if (hasHit) return;

        velocity += Physics.gravity * 0.3f * Time.deltaTime;
        float moveStep = velocity.magnitude * Time.deltaTime;
        Ray ray = new Ray(transform.position, velocity.normalized);

        if (Physics.Raycast(ray, out RaycastHit hit, moveStep, hitLayers, QueryTriggerInteraction.Ignore))
        {
            HitSurface(hit);
        }
        else
        {
            transform.position += velocity * Time.deltaTime;
        }
    }

    private void HitSurface(RaycastHit hit)
    {
        hasHit = true;

        if (hit.collider.CompareTag("Player") ||
            hit.collider.GetComponentInParent<PlayerCharacter>() != null ||
            hit.collider.CompareTag("Enemy"))
        {
            Destroy(gameObject);
            return;
        }

        if (trapPrefab != null)
        {
            Vector3 splatPos = hit.point + hit.normal * surfaceOffset;
            Quaternion splatRotation = Quaternion.FromToRotation(Vector3.up, hit.normal);

            if (MusicManager.Instance != null)
            {
                MusicManager.Instance.PlayJamHitSound(hit.point);
            }

            GameObject splat = Instantiate(trapPrefab, splatPos, splatRotation);

            if (!hit.collider.CompareTag("Player") && hit.collider.GetComponentInParent<PlayerCharacter>() == null)
            {
                splat.transform.SetParent(hit.collider.transform);
            }
        }

        Destroy(gameObject);
    }
}