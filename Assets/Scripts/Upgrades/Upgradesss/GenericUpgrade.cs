using Unity.Jobs;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "New Upgrade", menuName = "Upgades/Generic")]

public class GenericUpgrade : UpgradeBase
{
    [SerializeField] bool isAttack = false;
    [SerializeField]  bool isSpeed = false;
    [SerializeField] bool isCooldown = false;


    public override void Upgrade()
    {
        if (isAttack)
        {
            FindFirstObjectByType<PlayerHealthAndStat>().GetComponent<PlayerHealthAndStat>().damage *= 1.15f;
            FindFirstObjectByType<PlayerHealthAndStat>().GetComponent<PlayerHealthAndStat>().AttackNormal();

        }
        if (isSpeed)
        {
            FindFirstObjectByType<PlayerHealthAndStat>().GetComponent<PlayerHealthAndStat>().maxSpeed *= 1.1f;
            FindFirstObjectByType<PlayerHealthAndStat>().GetComponent<PlayerHealthAndStat>().SpeedNormal();

        }
        if (isCooldown)
        {
            FindFirstObjectByType<PlayerHealthAndStat>().GetComponent<PlayerHealthAndStat>().cooldownMultiplier -= 0.15f;
        }
    }
}
