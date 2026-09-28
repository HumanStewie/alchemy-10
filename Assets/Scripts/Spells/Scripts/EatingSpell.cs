using UnityEngine;

[CreateAssetMenu(fileName = "EatingSpell", menuName = "Spells/EatingSpell")]
public class EatingSpell : SpellTemplate
{
    public bool isTier11 = false;
    public bool isTier21 = false;
    public bool isTier12 = false;
    public bool isTier22 = false;

    public override void Cast(GameObject caster, Vector3 targetPoint, float scale)
    {
        if (BookMovement.Instance != null)
        {
            BookMovement.Instance.EatAnimation(() =>
            {
                ApplyEatEffects();
            });
        }
        else
        {
            ApplyEatEffects();
        }
    }

    private void ApplyEatEffects()
    {
        var playerHealth = Object.FindFirstObjectByType<PlayerHealthAndStat>();
        if (playerHealth != null)
        {
            playerHealth.heal(20);
        }
        MusicManager.Instance.PlayEatingSound(Object.FindFirstObjectByType<PlayerCharacter>()?.transform.position ?? Vector3.zero);
        if (GameManager.Instance != null)
        {
            if (isTier11) GameManager.Instance.SlowEVERYTHING(5, 50);
            if (isTier21) GameManager.Instance.SlowEVERYTHING(8, 50);
            if (isTier12) GameManager.Instance.SlowEVERYTHING(3, 75);
            if (isTier22) GameManager.Instance.SlowEVERYTHING(5, 75);
        }
    }
}