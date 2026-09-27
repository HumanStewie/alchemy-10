using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "EatingSpell", menuName = "Spells/EatingSpell")]
public class EatingSpell : SpellTemplate
{
    public GameObject wall;
    public bool isTier11 = false;
    public bool isTier21 = false;
    public bool isTier12 = false;
    public bool isTier22 = false;

    public override void Cast(GameObject caster, Vector3 targetPoint, float scale)
    {
        BookMovement.Instance.WallSpellAnimation();
        FindFirstObjectByType<PlayerHealthAndStat>().heal(20);
        if (isTier11)
        {
            GameManager.Instance.SlowEVERYTHING(5, 50);
        }
        if (isTier21)
        {
            GameManager.Instance.SlowEVERYTHING(8, 50);
        }
        if (isTier12)
        {
            GameManager.Instance.SlowEVERYTHING(3, 75);
        }
        if (isTier22)
        {
            GameManager.Instance.SlowEVERYTHING(5, 75);
        }
    }

}