using UnityEngine;

public class SpellBranchManager : MonoBehaviour
{
    [Header("4 Spell Nodes")]
    [SerializeField] private SmallSpell node1_1;
    [SerializeField] private SmallSpell node1_2;
    [SerializeField] private SmallSpell node2_1;
    [SerializeField] private SmallSpell node2_2;

    private void Start()
    {
        RefreshStates();
    }

    public void OnNodePurchased(SmallSpell.SpellNodeID purchasedID)
    {
        switch (purchasedID)
        {
            case SmallSpell.SpellNodeID.Node_1_1:
                node1_1.isPurchased = true;
                break;
            case SmallSpell.SpellNodeID.Node_1_2:
                node1_2.isPurchased = true;
                break;
            case SmallSpell.SpellNodeID.Node_2_1:
                node2_1.isPurchased = true;
                break;
            case SmallSpell.SpellNodeID.Node_2_2:
                node2_2.isPurchased = true;
                break;
        }

        RefreshStates();

        // Close the panel and go to next wave after 1 upgrade
        CloseUpgradeMenu();
    }

    public void RefreshStates()
    {
        bool bought1_1 = node1_1 != null && node1_1.isPurchased;
        bool bought1_2 = node1_2 != null && node1_2.isPurchased;
        bool bought2_1 = node2_1 != null && node2_1.isPurchased;
        bool bought2_2 = node2_2 != null && node2_2.isPurchased;

        bool inTier1Branch = bought1_1 || bought1_2;
        bool inTier2Branch = bought2_1 || bought2_2;

        if (node1_1 != null)
            node1_1.SetState(unlocked: !inTier2Branch, purchased: bought1_1);

        if (node1_2 != null)
            node1_2.SetState(unlocked: bought1_1 && !inTier2Branch, purchased: bought1_2);

        if (node2_1 != null)
            node2_1.SetState(unlocked: !inTier1Branch, purchased: bought2_1);

        if (node2_2 != null)
            node2_2.SetState(unlocked: bought2_1 && !inTier1Branch, purchased: bought2_2);
    }

    private void CloseUpgradeMenu()
    {
        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas != null)
        {
            canvas.gameObject.SetActive(false);
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.GoNextWave();
        }
    }
}