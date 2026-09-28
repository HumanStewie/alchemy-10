using UnityEngine;

[CreateAssetMenu(fileName = "TimeSlowUpgrade", menuName = "Upgrades/TimeSlow")]
public class TimeSlowUpgrade : UpgradeBase
{
    public enum TimeSlowBranch
    {
        Tier11_5s50,   // isTier11
        Tier12_3s75,   // isTier12
        Tier21_8s50,   // isTier21
        Tier22_5s75    // isTier22
    }

    public TimeSlowBranch selectedBranch;

    public override void Upgrade()
    {
        EatingSpell eating = GestureRecognizer.Instance.templates.Find(s => s is EatingSpell) as EatingSpell;
        if (eating == null) return;

        eating.isTier11 = false;
        eating.isTier12 = false;
        eating.isTier21 = false;
        eating.isTier22 = false;

        switch (selectedBranch)
        {
            case TimeSlowBranch.Tier11_5s50:
                eating.isTier11 = true;
                break;
            case TimeSlowBranch.Tier12_3s75:
                eating.isTier12 = true;
                break;
            case TimeSlowBranch.Tier21_8s50:
                eating.isTier21 = true;
                break;
            case TimeSlowBranch.Tier22_5s75:
                eating.isTier22 = true;
                break;
        }
    }
}