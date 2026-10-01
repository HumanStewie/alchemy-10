using DG.Tweening;
using UnityEngine;

public enum SwordState
{
    Pull,
    SwingLeft,
    SwingRight,
    Stop
}

[CreateAssetMenu(fileName = "SwordSpell", menuName = "Spells/SwordSpell")]
public class SwordSpell : SpellTemplate
{
    [Header("Slash Prefab")]
    public GameObject slashPrefab;

    [Header("Base Stats")]
    public float baseDamage = 35f;
    public float knockbackForce = 22f;
    public float collisionDamage = 20f;
    public float rangeMultiplier = 1f;
    public bool attackTwice = false;

    [Header("Slash Movement")]
    [SerializeField] private float travelDistance = 0.8f;
    [SerializeField] private float travelDuration = 0.35f;

    private bool isHoldingSword = false;
    private bool swingLeft = false;

    private readonly Vector3 leftLocalPos = new Vector3(-0.12f, 1.05f, 2.59f);
    private readonly Vector3 leftLocalRot = new Vector3(0f, 180f, -30f);

    private readonly Vector3 rightLocalPos = new Vector3(0.43f, 1.05f, 2.59f);
    private readonly Vector3 rightLocalRot = new Vector3(0f, 180f, 30f);

    public void ResetSwordState()
    {
        isHoldingSword = false;
        swingLeft = false;
    }

    public override void Cast(GameObject caster, Vector3 targetPoint, float scale)
    {
        if (BookMovement.Instance != null)
        {
            if (!BookMovement.Instance.IsHoldingSword)
            {
                isHoldingSword = false;
            }

            // Swinging
            if (isHoldingSword)
            {
                if (attackTwice) swingLeft = !swingLeft;

                SwordState nextState = swingLeft ? SwordState.SwingLeft : SwordState.SwingRight;
                BookMovement.Instance.JamSwordSwing(nextState, () =>
                {
                    ExecuteSlash(caster, swingLeft);
                });
            }
            // Pulling out
            else
            {
                BookMovement.Instance.JamSwordSwing(SwordState.Pull, () =>
                {
                    isHoldingSword = true;
                });
            }
        }
        else
        {
            ExecuteSlash(caster, swingLeft);
        }
    }

    private void ExecuteSlash(GameObject caster, bool isLeft)
    {
        PlayerCharacter player = FindFirstObjectByType<PlayerCharacter>();
        if (player == null) return;

        Transform playerTransform = player.transform;

        Vector3 targetLocalPos = isLeft ? leftLocalPos : rightLocalPos;
        Vector3 targetLocalRot = isLeft ? leftLocalRot : rightLocalRot;

        Vector3 spawnOrigin = playerTransform.TransformPoint(targetLocalPos);
        Quaternion slashRotation = playerTransform.rotation * Quaternion.Euler(targetLocalRot);

        GameObject slashObj = Instantiate(slashPrefab, spawnOrigin, slashRotation);

        Vector3 moveTarget = slashObj.transform.position + playerTransform.forward * travelDistance;
        slashObj.transform.DOMove(moveTarget, travelDuration).SetEase(Ease.OutSine);

        if (MusicManager.Instance != null)
        {
            MusicManager.Instance.PlaySwordSound(spawnOrigin);
        }

        if (CameraShake.Instance != null)
        {
            CameraShake.Instance.ShakeLight();
        }

        if (slashObj.TryGetComponent<SwordSlash>(out var slashScript))
        {
            slashScript.Initialize(baseDamage, knockbackForce, collisionDamage, rangeMultiplier, attackTwice);
        }
    }
}