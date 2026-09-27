using UnityEngine;

[CreateAssetMenu(fileName = "WallUpgrade", menuName = "Upgrades/WallUpgrade")]
public class WallUpgrade : UpgradeBase
{
    public enum WallBranch
    {
        DoubleWall,       
        DamagingWall,      
        GiganticClimbWall,
        PocketHealWall     
    }

    public WallBranch selectedBranch;

    public override void Upgrade()
    {
        WallSpell wallSpell = GestureRecognizer.Instance.templates.Find(s => s is WallSpell) as WallSpell;
        if (wallSpell == null) return;

        switch (selectedBranch)
        {
            case WallBranch.DoubleWall:
                wallSpell.maxWalls = 2;
                break;

            case WallBranch.DamagingWall:
                wallSpell.contactDamage = 15f;
                break;

            case WallBranch.GiganticClimbWall:
                wallSpell.sizeMultiplier = 1.5f;
                wallSpell.allowWallWalk = true;
                break;

            case WallBranch.PocketHealWall:
                wallSpell.sizeMultiplier = 0.7f;
                wallSpell.runeHeals = true;
                break;
        }
    }
}