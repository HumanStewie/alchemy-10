using UnityEngine;

[CreateAssetMenu(fileName = "BazookaUpgrade", menuName = "Upgrades/BazookaUpgrade")]
public class BazookaUpgrade : UpgradeBase
{
    public enum BazookaBranch
    {
        IncreaseRangeAndDamage, 
        Freeze3s,                
        DoubleShotSmallerBlast,  
        Freeze2s                 
    }

    public BazookaBranch selectedBranch;

    public override void Upgrade()
    {
        ThrowJam bazooka = GestureRecognizer.Instance.templates.Find(s => s is ThrowJam) as ThrowJam;
        if (bazooka == null) return;

        switch (selectedBranch)
        {
            case BazookaBranch.IncreaseRangeAndDamage:
                bazooka.damage += 25f;
                bazooka.blastRadius *= 1.4f;
                break;

            case BazookaBranch.Freeze3s:
                bazooka.freezeDuration = 3f;
                break;

            case BazookaBranch.DoubleShotSmallerBlast:
                bazooka.shotsPerCast = 2;
                bazooka.blastRadius *= 0.75f; 
                break;

            case BazookaBranch.Freeze2s:
                bazooka.freezeDuration = 2f;
                break;
        }
    }
}