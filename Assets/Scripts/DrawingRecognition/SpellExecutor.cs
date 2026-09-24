using System.Runtime.CompilerServices;
using UnityEngine;

public class SpellExecutor : MonoBehaviour
{
    public static SpellExecutor Instance;


    private void Awake()
    {
        Instance = this;
    }

    public void ExecuteSpell(SpellTemplate spell)
    {
        Debug.Log(spell);
    }
}
