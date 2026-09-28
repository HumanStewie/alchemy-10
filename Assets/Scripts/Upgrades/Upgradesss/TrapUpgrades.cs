using UnityEngine;

public enum TrapBranch
{
    PoisonousTrap,   // isTier11
    EvolvedTrap,     // isTier21
    BurningTrap,     // isTier12 (also bigger + longer)
    RagebaitedTrap   // isTier22 (death explosion)
}

[CreateAssetMenu(fileName = "TrapUpgrade", menuName = "Upgrades/Trap")]
public class TrapUpgrade : UpgradeBase
{
    public TrapBranch selectedUpgrade;

    public override void Upgrade()
    {
        BreadTrap trap = GestureRecognizer.Instance.templates.Find(s => s is BreadTrap) as BreadTrap;
        if (trap == null) return;


        switch (selectedUpgrade)
        {
            case TrapBranch.PoisonousTrap:
                trap.isTier11 = true;
                break;
            case TrapBranch.EvolvedTrap:
                trap.isTier21 = true;
                break;
            case TrapBranch.BurningTrap:
                trap.isTier12 = true;
                break;
            case TrapBranch.RagebaitedTrap:
                trap.isTier22 = true;
                break;
        }
    }
}