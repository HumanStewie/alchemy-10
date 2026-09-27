using DG.Tweening;
using UnityEngine;

public class WallProp : MonoBehaviour
{
    [Header("Wall Properties")]
    public float currentHealth = 100f;
    public float maxHealth = 100f;
    public float damageOnTouch = 0f;
    public bool isStickyWallWalk = false;
    public bool healOnRuneTouch = false;
    public float runeHealAmount = 25f;

    private Tween moveTween;
    private bool isDespawning = false;

    public void Initialize(float lifetime, float damage, float sizeMultiplier, bool allowWallWalk, bool runeHeals)
    {
        damageOnTouch = damage;
        isStickyWallWalk = allowWallWalk;
        healOnRuneTouch = runeHeals;
        transform.localScale *= sizeMultiplier;

        Invoke(nameof(DespawnWall), lifetime);
    }

    public void DespawnWall()
    {
        if (isDespawning) return;
        isDespawning = true;

        CancelInvoke(nameof(DespawnWall));
        moveTween?.Kill();

        transform.DOMoveY(transform.position.y - 3f, 0.4f).SetEase(Ease.InSine);
        transform.DOScale(Vector3.zero, 0.4f).SetEase(Ease.InBack).OnComplete(() =>
        {
            Destroy(gameObject);
        });
    }

    private void OnTriggerEnter(Collider other)
    {
        if (damageOnTouch > 0f && other.CompareTag("Enemy"))
        {
            other.SendMessageUpwards("takeDamage", damageOnTouch, SendMessageOptions.DontRequireReceiver);
        }

        if (healOnRuneTouch && other.CompareTag("Player"))
        {
            var health = other.GetComponent<PlayerHealthAndStat>();
            if (health != null)
            {
                health.heal(runeHealAmount);
            }
        }

        if (isStickyWallWalk && other.CompareTag("Player"))
        {
            other.SendMessageUpwards("EnableWallStick", true, SendMessageOptions.DontRequireReceiver);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (isStickyWallWalk && other.CompareTag("Player"))
        {
            other.SendMessageUpwards("EnableWallStick", false, SendMessageOptions.DontRequireReceiver);
        }
    }
}