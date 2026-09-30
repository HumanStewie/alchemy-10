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

    private bool isHoldingSword;
    private bool swingLeft = false;

    // Resets internal sword state when sword is stowed away into the jar
    public void ResetSwordState()
    {
        isHoldingSword = false;
        swingLeft = false;
    }

    public override void Cast(GameObject caster, Vector3 targetPoint, float scale)
    {
        if (BookMovement.Instance != null)
        {
            // If BookMovement is no longer in sword stance (e.g. stowed when drawing a new spell), keep in sync
            if (!BookMovement.Instance.IsHoldingSword)
            {
                isHoldingSword = false;
            }

            // Swinging
            if (isHoldingSword)
            {
                if(attackTwice) swingLeft = !swingLeft;
                BookMovement.Instance.JamSwordSwing(swingLeft ? SwordState.SwingLeft : SwordState.SwingRight, () =>
                {
                    ExecuteSlash(caster, targetPoint);
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
            ExecuteSlash(caster, targetPoint);
        }
    }

    private void ExecuteSlash(GameObject caster, Vector3 targetPoint)
    {
        Vector3 spawnOrigin = caster.transform.position + caster.transform.forward * 1.2f + Vector3.up * 0.5f;
        Quaternion slashRotation = Quaternion.LookRotation(caster.transform.forward);

        GameObject slashObj = Instantiate(slashPrefab, spawnOrigin, slashRotation);
        MusicManager.Instance.PlaySwordSound(spawnOrigin);
        CameraShake.Instance.ShakeLight();
        if (slashObj.TryGetComponent<SwordSlash>(out var slashScript))
        {
            slashScript.Initialize(baseDamage, knockbackForce, collisionDamage, rangeMultiplier, attackTwice);
        }
    }
}