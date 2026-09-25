using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "WallSpell", menuName = "Spells/WallSpell")]

public class WallSpell : SpellTemplate
{
    public GameObject wall;
    public override void Cast(GameObject caster, Vector3 targetPoint, float scale)
    {
        BookMovement.Instance.WallSpellAnimation();
        Instantiate(wall, caster.transform.position - new Vector3(0, 10f, 0), Quaternion.identity);
    }
}
