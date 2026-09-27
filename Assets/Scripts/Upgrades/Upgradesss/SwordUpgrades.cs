using UnityEngine;

[CreateAssetMenu(fileName = "SwordUpgrade", menuName = "Upgrades/SwordUpgrade")]
public class SwordUpgrade : UpgradeBase
{
    public enum SwordBranch
    {
        MassiveKnockbackWithCollision, 
        MoreCollisionDamage,         
        FurtherMeleeRange,          
        AttackTwice                  
    }

    public SwordBranch selectedBranch;

    public override void Upgrade()
    {
        SwordSpell sword = GestureRecognizer.Instance.templates.Find(s => s is SwordSpell) as SwordSpell;
        if (sword == null) return;

        switch (selectedBranch)
        {
            case SwordBranch.MassiveKnockbackWithCollision:
                sword.knockbackForce = 35f;
                sword.collisionDamage = 25f;
                break;

            case SwordBranch.MoreCollisionDamage:
                sword.collisionDamage += 35f;
                break;

            case SwordBranch.FurtherMeleeRange:
                sword.rangeMultiplier = 1.6f;
                break;

            case SwordBranch.AttackTwice:
                sword.attackTwice = true;
                break;
        }
    }
}