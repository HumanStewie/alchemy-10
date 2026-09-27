using Unity.Collections;
using Unity.Jobs;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "New Upgrade", menuName = "Upgades/TimeSlow")]

public class TimeSlowUpgrade : UpgradeBase
{
    bool t11;
    bool t12;
    bool t21;
    bool t22;
        
    public override void Upgrade()
    {
        EatingSpell eating = GestureRecognizer.Instance.templates.Find(s => s is EatingSpell) as EatingSpell;
        if (t11)
        {
            eating.isTier11 = true;
        }
        if (t21)
        {
            eating.isTier21 = true;
            eating.isTier11 = false;
        }
        if (t12)
        {
            eating.isTier12 = true;
        }
        if (t22)
        {
            eating.isTier12 = false;
            eating.isTier22 = true;
        }
    }
}
