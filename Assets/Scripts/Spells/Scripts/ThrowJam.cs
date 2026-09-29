using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ThrowJam", menuName = "Spells/ThrowJam")]
public class ThrowJam : SpellTemplate
{
    [Header("Base Projectile")]
    public GameObject bazookaBlobPrefab;
    public float launchSpeed = 22f;

    [Header("Base Stats")]
    public float damage = 40f;
    public float blastRadius = 3.5f;
    public float freezeDuration = 0f;
    public int shotsPerCast = 1;
    public float burstDelay = 0.18f;

    public override void Cast(GameObject caster, Vector3 targetPoint, float scale)
    {
        if (BookMovement.Instance != null)
        {
            BookMovement.Instance.SwingJamAttack(() =>
            {
                // Uses caster's MonoBehaviour to safely run the burst coroutine
                caster.GetComponent<MonoBehaviour>().StartCoroutine(FireBurst(targetPoint));
            });
        }
    }

    private IEnumerator FireBurst(Vector3 targetPoint)
    {
        for (int i = 0; i < shotsPerCast; i++)
        {
            FireSingleBlob(targetPoint);
            if (shotsPerCast > 1 && i < shotsPerCast - 1)
            {
                yield return new WaitForSeconds(burstDelay);
            }
        }
    }

    private void FireSingleBlob(Vector3 targetPoint)
    {
        Vector3 spawnOrigin = Camera.main.transform.position + (Camera.main.transform.forward * 0.4f);
        Vector3 dir = (targetPoint - spawnOrigin).normalized;
        MusicManager.Instance.PlayBazookaSound(spawnOrigin);
        GameObject blob = Instantiate(bazookaBlobPrefab, spawnOrigin, Quaternion.identity);
        CameraShake.Instance.ShakeHeavy();
        if (blob.TryGetComponent<BazookaLaunch>(out var blobScript))
        {
            blobScript.Launch(dir, launchSpeed, damage, blastRadius, freezeDuration);
        }
    }
}