using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[CreateAssetMenu(fileName = "WallSpell", menuName = "Spells/WallSpell")]

public class WallSpell : SpellTemplate
{
    public GameObject wall;
    public float yFinal = 2.75f;
    public override void Cast(GameObject caster, Vector3 targetPoint, float scale)
    {
        BookMovement.Instance.WallSpellAnimation();
        GameObject walls =  Instantiate(wall, FindAnyObjectByType<BookMovement>().transform.position - new Vector3(0, 10f, 0), Quaternion.Euler(-90,FindAnyObjectByType<PlayerCharacter>().transform.eulerAngles.y -90f, 0));
        walls.transform.DOMoveY(yFinal, 0.5f).SetEase(Ease.InOutSine);
    }
}
