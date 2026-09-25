using UnityEngine;
[CreateAssetMenu(fileName = " BreadTrap", menuName = "Spells/BreadTrap")]

public class BreadTrap : SpellTemplate
{
    public GameObject wall;
    public override void Cast(GameObject caster, Vector3 targetPoint, float scale)
    {
        BookMovement.Instance.WallSpellAnimation();
        Instantiate(wall, caster.transform.position - new Vector3(0, 10f, 0), Quaternion.identity);
    }
}