using UnityEngine;


[CreateAssetMenu(fileName = "Sword", menuName = "Spells/Sword")]
public class SwordSpell : SpellTemplate
{
    public GameObject wall;
    public override void Cast(GameObject caster, Vector3 targetPoint, float scale)
    {
        BookMovement.Instance.WallSpellAnimation();
        Instantiate(wall, caster.transform.position - new Vector3(0, 10f, 0), Quaternion.identity);
    }
}
