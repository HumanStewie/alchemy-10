using UnityEngine;
using UnityEngine.Rendering.Universal;
using DG.Tweening;

[System.Flags]
public enum JamProperties
{
    None = 0,
    Sticky = 1 << 0,
    Slippery = 1 << 1
}

[RequireComponent(typeof(DecalProjector))]
[RequireComponent(typeof(BoxCollider))]
public class JamSpread : MonoBehaviour
{
    [Header("Behavior Preset")]
    [Tooltip("Select one or both behaviors in the dropdown")]
    [SerializeField] private JamProperties jamProperties = JamProperties.Sticky;

    [Header("Life Cycle")]
    [SerializeField] private float duration = 8f;
    [SerializeField] private float fadeDuration = 1.2f;

    [Header("Pop Animation")]
    [SerializeField] private float popDuration = 0.1f;

    [Header("Trigger Settings")]
    [SerializeField] private float triggerThickness = 0.15f;

    [Header("Sticky Settings")]
    [Range(0.05f, 0.95f)]
    [SerializeField] private float slowMultiplier = 0.4f;
    [SerializeField] private bool pinToWallIfVertical = true;

    [Header("Slippery Settings")]
    [SerializeField] private float slipForce = 12f;
    [Tooltip("If true, projects along the decal's forward plane")]
    [SerializeField] private bool useDecalDirection = true;
    [SerializeField] private Vector3 customFlowDirection = Vector3.forward;

    private DecalProjector projector;
    private BoxCollider triggerBox;
    private Vector3 targetSize;
    private bool isWallSurface;

    public bool IsSticky => (jamProperties & JamProperties.Sticky) != 0;
    public bool IsSlippery => (jamProperties & JamProperties.Slippery) != 0;

    private void Awake()
    {
        PlayerCharacter player = FindAnyObjectByType<PlayerCharacter>();
        float playerYaw = player != null ? player.transform.eulerAngles.y : 0f;
        transform.rotation = Quaternion.Euler(90f, playerYaw, 0f);

        projector = GetComponent<DecalProjector>();
        triggerBox = GetComponent<BoxCollider>();

        gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");

        triggerBox.isTrigger = true;
        targetSize = projector.size;

        float angleFromDown = Vector3.Angle(transform.forward, Vector3.down);
        isWallSurface = angleFromDown > 45f;

        triggerBox.size = new Vector3(targetSize.x, targetSize.y, triggerThickness);
        triggerBox.center = Vector3.zero;
    }

    public void InitSplat()
    {
        projector.size = new Vector3(0f, 0f, targetSize.z);
        triggerBox.size = new Vector3(0f, 0f, triggerThickness);

        DOTween.To(() => projector.size, s =>
        {
            projector.size = s;
            triggerBox.size = new Vector3(s.x, s.y, triggerThickness);
        }, targetSize, popDuration).SetEase(Ease.OutBack);

        DOVirtual.DelayedCall(Mathf.Max(0.1f, duration - fadeDuration), StartFadeOut);
    }

    private void StartFadeOut()
    {
        if (triggerBox != null)
        {
            triggerBox.enabled = false;
        }

        DOTween.To(() => projector.fadeFactor, f => projector.fadeFactor = f, 0f, fadeDuration)
            .OnComplete(() => Destroy(gameObject));
    }

    public Vector3 GetFlowDirection()
    {
        Vector3 dir = useDecalDirection ? transform.up : customFlowDirection;
        dir.y = 0f;

        if (dir.sqrMagnitude < 0.001f)
        {
            dir = transform.forward;
            dir.y = 0f;
        }

        return dir.normalized;
    }

    // --- GAMEPLAY TRIGGERS ---

    private void OnTriggerEnter(Collider other)
    {
        // 1. SLIPPERY LOGIC
        if (IsSlippery)
        {
            // Redirect rune course
            RuneController rune = other.GetComponentInParent<RuneController>();
            if (rune != null)
            {
                rune.Redirect(GetFlowDirection());
            }

            // Slide physics objects
            if (other.attachedRigidbody != null)
            {
                other.attachedRigidbody.AddForce(GetFlowDirection() * slipForce, ForceMode.VelocityChange);
            }
        }

        // 2. STICKY LOGIC
        if (IsSticky)
        {
            if (other.CompareTag("Enemy"))
            {
                // Silently notifies your enemy script if it has an ApplySlow function, otherwise ignores
                other.SendMessageUpwards("ApplySlow", slowMultiplier, SendMessageOptions.DontRequireReceiver);

                // If enemy moves via Rigidbody physics
                if (other.attachedRigidbody != null)
                {
                    other.attachedRigidbody.linearVelocity *= slowMultiplier;
                }
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (IsSlippery && other.attachedRigidbody != null)
        {
            other.attachedRigidbody.AddForce(GetFlowDirection() * (slipForce * 0.5f * Time.deltaTime), ForceMode.VelocityChange);
        }

        if (IsSticky && isWallSurface && pinToWallIfVertical && other.attachedRigidbody != null)
        {
            other.attachedRigidbody.linearVelocity = Vector3.zero;
            other.attachedRigidbody.useGravity = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (IsSticky)
        {
            if (other.CompareTag("Enemy"))
            {
                // Silently notifies your enemy script to reset speed
                other.SendMessageUpwards("RemoveSlow", SendMessageOptions.DontRequireReceiver);
            }

            // Restore gravity if pinned to a wall
            if (isWallSurface && other.attachedRigidbody != null)
            {
                other.attachedRigidbody.useGravity = true;
            }
        }
    }
}