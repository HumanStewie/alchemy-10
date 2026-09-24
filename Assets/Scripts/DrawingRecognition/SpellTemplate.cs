using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName ="SpellTemp",menuName ="SpellTemp")]
public abstract class SpellTemplate : ScriptableObject
{
    public string spellName;
    public float baseManaCost = 20f;
    public float cd = 0.2f;
    public List<Vector2> points = new List<Vector2>();

    public abstract void Cast(GameObject caster, Vector3 targetPoint, float scale);
}
