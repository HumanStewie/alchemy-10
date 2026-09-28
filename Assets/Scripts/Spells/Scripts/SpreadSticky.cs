using UnityEngine;

[CreateAssetMenu(fileName = "SpreadSlippery", menuName = "Spells/SpreadSlippery")]
public class SpreadSticky : SpellTemplate
{
    [Header("Projectile Settings")]
    [SerializeField] private GameObject jamBlobPrefab;
    [SerializeField] private float projectileSpeed = 28f;

    public override void Cast(GameObject caster, Vector3 targetPoint, float scale)
    {
        if (BookMovement.Instance != null)
        {
            BookMovement.Instance.SpreadCardThrowAnimation(() =>
            {
                ShootSingleJam(targetPoint);
            });
        }
        else
        {
            ShootSingleJam(targetPoint);
        }
    }

    private void ShootSingleJam(Vector3 targetPoint)
    {
        if (Camera.main == null || jamBlobPrefab == null) return;

        Vector3 spawnOrigin = Camera.main.transform.position + (Camera.main.transform.forward * 0.4f);
        Vector3 shootDir = (targetPoint - spawnOrigin).normalized;
        MusicManager.Instance.PlayJamThrowSound(spawnOrigin);

        GameObject blob = Instantiate(jamBlobPrefab, spawnOrigin, Quaternion.identity);
        if (blob.TryGetComponent<JamAmmo>(out var jamScript))
        {
            jamScript.Launch(shootDir, projectileSpeed);
        }
    }
}