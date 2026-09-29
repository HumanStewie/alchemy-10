using UnityEngine;

[CreateAssetMenu(fileName = "BreadTrap", menuName = "Spells/BreadTrap")]
public class BreadTrap : SpellTemplate
{
    [Header("Projectile Settings")]
    [SerializeField] private GameObject jamBlobPrefab;
    [SerializeField] private float projectileSpeed = 28f;

    public bool isTier11;
    public bool isTier12;
    public bool isTier21;
    public bool isTier22;

    public override void Cast(GameObject caster, Vector3 targetPoint, float scale)
    {
        if (BookMovement.Instance != null)
        {
            BookMovement.Instance.ThrowTrapAnimation(() =>
            {
                ShootSingleJam(targetPoint, caster);
            });
        }
        else
        {
            ShootSingleJam(targetPoint, caster);
        }
    }

    private void ShootSingleJam(Vector3 targetPoint, GameObject caster)
    {
        if (Camera.main == null || jamBlobPrefab == null) return;

        Vector3 spawnOrigin = Camera.main.transform.position + (Camera.main.transform.forward * 0.8f);
        Vector3 shootDir = (targetPoint - spawnOrigin).normalized;

        if (MusicManager.Instance != null)
        {
            MusicManager.Instance.PlayJamThrowSound(spawnOrigin);
        }

        GameObject blob = Instantiate(jamBlobPrefab, spawnOrigin, Quaternion.identity);

        if (caster != null)
        {
            Collider[] playerColliders = caster.GetComponentsInChildren<Collider>();
            Collider blobCollider = blob.GetComponent<Collider>();

            if (blobCollider != null)
            {
                foreach (Collider playerCol in playerColliders)
                {
                    Physics.IgnoreCollision(blobCollider, playerCol, true);
                }
            }
        }

        if (blob.TryGetComponent<ProjectileGetter>(out var jamScript))
        {
            jamScript.Launch(shootDir, projectileSpeed);
        }
    }
}