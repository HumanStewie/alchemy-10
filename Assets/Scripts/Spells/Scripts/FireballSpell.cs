using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName ="Fuckignfireball", menuName ="firebacllfuck")]
public class FireballSpell : SpellTemplate
{
    public GameObject fire;
    public override void Cast(GameObject caster, Vector3 targetPoint, float scale)
    {
        Instantiate(fire);
    }
}
