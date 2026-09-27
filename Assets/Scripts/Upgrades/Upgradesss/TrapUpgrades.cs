using Unity.Collections;
using Unity.Jobs;
using UnityEditor;
using UnityEngine;

public enum WallBranch
{
    PoisonousTrap,
    EvolvedTrap,
    BurningTrap,
    RagebaitedTrap
}

[CreateAssetMenu(fileName = "New Upgrade", menuName = "Upgades/Trap")]

public class TrapUpgrade : UpgradeBase
{
    public WallBranch selectedUpgrade;
    public override void Upgrade()
    {
        BreadTrap eating = GestureRecognizer.Instance.templates.Find(s => s is BreadTrap) as BreadTrap;
        if (selectedUpgrade == WallBranch.PoisonousTrap)
        {
            eating.isTier11 = true;
        }
        if (selectedUpgrade == WallBranch.EvolvedTrap)
        {
            eating.isTier21 = true;
        }
        if (selectedUpgrade == WallBranch.BurningTrap)
        {
            eating.isTier12 = true;
        }
        if (selectedUpgrade == WallBranch.RagebaitedTrap)
        {
            eating.isTier22 = true;
        }
    }
}
