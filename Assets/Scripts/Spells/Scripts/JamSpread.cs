using UnityEngine;
using UnityEngine.Rendering.Universal;
using DG.Tweening;

[RequireComponent(typeof(DecalProjector))]
[RequireComponent(typeof(BoxCollider))]
public class JamSpread : MonoBehaviour
{
    [Header("Life Cycle")]
    [SerializeField] private float duration = 8f;
    [SerializeField] private float fadeDuration = 1.2f;

    [Header("Pop Animation")]
    [SerializeField] private float popDuration = 0.1f;

    [Header("Gameplay Effects")]
    [SerializeField] private float slowMultiplier = 0.4f;

    private DecalProjector projector;
    private BoxCollider triggerBox;
    private Vector3 targetSize;

    void Awake()
    {
        projector = GetComponent<DecalProjector>();
        triggerBox = GetComponent<BoxCollider>();

        triggerBox.isTrigger = true;
        targetSize = projector.size;
        triggerBox.size = targetSize;
    }

    public void InitSplat()
    {
        projector.size = new Vector3(0f, 0f, targetSize.z);
        triggerBox.size = new Vector3(0f, 0f, targetSize.z);

        DOTween.To(() => projector.size, s =>
        {
            projector.size = s;
            triggerBox.size = s;
        }, targetSize, popDuration).SetEase(Ease.OutBack);

        DOVirtual.DelayedCall(duration - fadeDuration, StartFadeOut);
    }

    private void StartFadeOut()
    {
        if (triggerBox != null) triggerBox.enabled = false;

        DOTween.To(() => projector.fadeFactor, f => projector.fadeFactor = f, 0f, fadeDuration)
            .OnComplete(() => Destroy(gameObject));
    }

    // --- GAMEPLAY TRIGGERS ---

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Debug.Log($"{other.name} entered sticky jam!");
            // other.GetComponent()?.ApplySlow(slowMultiplier);
        }

        if (other.CompareTag("Player"))
        {
            Debug.Log("Player touching sticky surface!");
            // other.GetComponent()?.SetWallCling(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            // other.GetComponent()?.RemoveSlow();
        }

        if (other.CompareTag("Player"))
        {
            // other.GetComponent()?.SetWallCling(false);
        }
    }
}