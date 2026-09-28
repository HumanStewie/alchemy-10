using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SpellUpgradeUI : MonoBehaviour
{
    [System.Serializable]
    public class UpgradeNode
    {
        public string id;
        public string displayName;
        [TextArea] public string description;
        public int tier;     
        public int cost = 1;
        public string flagName;   
        public Sprite icon;
        [HideInInspector] public bool purchased;
    }

    [System.Serializable]
    public class AttackBranch
    {
        public string attackName;
        public Sprite baseIcon;
        public SpellTemplate spellAsset;
        public UpgradeNode[] upgrades = new UpgradeNode[4];
    }

    [Header("Currency")]
    [SerializeField] private TextMeshProUGUI starCountText;

    [Header("Tabs (5 attacks)")]
    [SerializeField] private Button[] attackTabButtons;
    [SerializeField] private Image[] attackTabIcons;
    [SerializeField] private TextMeshProUGUI[] attackTabLabels;

    [Header("Tree UI")]
    [SerializeField] private Image baseAttackIcon;
    [SerializeField] private Button[] upgradeButtons;
    [SerializeField] private Image[] upgradeIcons;
    [SerializeField] private TextMeshProUGUI[] upgradeLabels;
    [SerializeField] private Image[] upgradeOwnedOverlay;

    [Header("Detail")]
    [SerializeField] private TextMeshProUGUI detailName;
    [SerializeField] private TextMeshProUGUI detailDesc;
    [SerializeField] private Button buyButton;
    [SerializeField] private TextMeshProUGUI buyButtonLabel;

    [Header("Go")]
    [SerializeField] private Button goButton;

    [Header("Branches")]
    [SerializeField] private List<AttackBranch> attacks = new List<AttackBranch>();

    private int currentStars;
    private int selectedAttack;
    private int selectedUpgrade = -1;

    private void Awake()
    {
        for (int i = 0; i < attackTabButtons.Length; i++)
        {
            int idx = i;
            attackTabButtons[i].onClick.AddListener(() => SelectAttack(idx));
        }

        for (int i = 0; i < upgradeButtons.Length; i++)
        {
            int idx = i;
            upgradeButtons[i].onClick.AddListener(() => SelectUpgrade(idx));
        }

        if (buyButton != null)
            buyButton.onClick.AddListener(TryBuySelected);

        if (goButton != null)
            goButton.onClick.AddListener(OnGo);
    }

    private void OnEnable()
    {
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        SelectAttack(0);
    }

    private void OnDisable()
    {
        Time.timeScale = 1f;
    }

    private void SelectAttack(int index)
    {
        if (index < 0 || index >= attacks.Count) return;
        selectedAttack = index;
        selectedUpgrade = -1;

        AttackBranch branch = attacks[index];

        if (baseAttackIcon != null)
            baseAttackIcon.sprite = branch.baseIcon;

        for (int i = 0; i < upgradeButtons.Length; i++)
        {
            bool has = i < branch.upgrades.Length && branch.upgrades[i] != null;
            upgradeButtons[i].gameObject.SetActive(has);
            if (!has) continue;

            UpgradeNode node = branch.upgrades[i];
            if (upgradeIcons != null && i < upgradeIcons.Length && upgradeIcons[i] != null)
                upgradeIcons[i].sprite = node.icon;
            if (upgradeLabels != null && i < upgradeLabels.Length && upgradeLabels[i] != null)
                upgradeLabels[i].text = node.displayName;

            bool owned = node.purchased;
            if (upgradeOwnedOverlay != null && i < upgradeOwnedOverlay.Length && upgradeOwnedOverlay[i] != null)
                upgradeOwnedOverlay[i].enabled = owned;

            upgradeButtons[i].interactable = !owned && CanUnlock(branch, i);
        }

        ClearDetail();
        HighlightTabs();
    }

    private void SelectUpgrade(int index)
    {
        AttackBranch branch = attacks[selectedAttack];
        if (index < 0 || index >= branch.upgrades.Length) return;

        selectedUpgrade = index;
        UpgradeNode node = branch.upgrades[index];

        if (detailName != null) detailName.text = node.displayName;
        if (detailDesc != null) detailDesc.text = node.description + "\n\nCost: " + node.cost + " ★";

        bool canBuy = !node.purchased && currentStars >= node.cost && CanUnlock(branch, index);
        if (buyButton != null)
        {
            buyButton.interactable = canBuy;
            if (buyButtonLabel != null)
                buyButtonLabel.text = node.purchased ? "OWNED" : (canBuy ? "BUY" : "LOCKED");
        }
    }

    private bool CanUnlock(AttackBranch branch, int upgradeIndex)
    {
        UpgradeNode node = branch.upgrades[upgradeIndex];
        if (node.tier <= 1) return true;

        for (int i = 0; i < branch.upgrades.Length; i++)
        {
            if (branch.upgrades[i] != null && branch.upgrades[i].tier == 1 && branch.upgrades[i].purchased)
                return true;
        }
        return false;
    }

    private void TryBuySelected()
    {
        if (selectedUpgrade < 0) return;

        AttackBranch branch = attacks[selectedAttack];
        UpgradeNode node = branch.upgrades[selectedUpgrade];

        if (node.purchased) return;
        if (currentStars < node.cost) return;
        if (!CanUnlock(branch, selectedUpgrade)) return;

        currentStars -= node.cost;
        node.purchased = true;
        ApplyFlag(branch.spellAsset, node.flagName);
        RefreshStarText();
        SelectAttack(selectedAttack);
        SelectUpgrade(selectedUpgrade);
    }

    private void ApplyFlag(SpellTemplate spell, string flag)
    {
        if (spell == null || string.IsNullOrEmpty(flag)) return;

        SpellTemplate live = spell;
        if (GestureRecognizer.Instance != null)
        {
            foreach (var t in GestureRecognizer.Instance.templates)
            {
                if (t != null && t.GetType() == spell.GetType())
                {
                    live = t;
                    break;
                }
            }
        }

        if (live is BreadTrap bread)
        {
            if (flag == "isTier11") bread.isTier11 = true;
            else if (flag == "isTier12") bread.isTier12 = true;
            else if (flag == "isTier21") bread.isTier21 = true;
            else if (flag == "isTier22") bread.isTier22 = true;
        }
        else if (live is EatingSpell eat)
        {
            if (flag == "isTier11") eat.isTier11 = true;
            else if (flag == "isTier12") eat.isTier12 = true;
            else if (flag == "isTier21") eat.isTier21 = true;
            else if (flag == "isTier22") eat.isTier22 = true;
        }
        else if (live is WallSpell wall)
        {
            if (flag == "maxWalls") wall.maxWalls += 1;
            else if (flag == "contactDamage") wall.contactDamage += 5f;
            else if (flag == "sizeMultiplier") wall.sizeMultiplier += 0.25f;
            else if (flag == "allowWallWalk") wall.allowWallWalk = true;
            else if (flag == "runeHeals") wall.runeHeals = true;
        }
        else if (live is SwordSpell sword)
        {
            if (flag == "damage") sword.baseDamage += 10f;
            else if (flag == "knockback") sword.knockbackForce += 5f;
            else if (flag == "range") sword.rangeMultiplier += 0.25f;
            else if (flag == "doubleHit") sword.attackTwice = true;
        }
        else if (live is ThrowJam jam)
        {
            if (flag == "damage") jam.damage += 10f;
            else if (flag == "radius") jam.blastRadius += 1f;
            else if (flag == "freeze") jam.freezeDuration += 1f;
            else if (flag == "shots") jam.shotsPerCast += 1;
        }
    }

    private void OnGo()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.GoNextWave();
        else
            gameObject.SetActive(false);
    }

    private void RefreshStarText()
    {
        if (starCountText != null)
            starCountText.text = "You have " + currentStars + " ★";
    }

    private void ClearDetail()
    {
        if (detailName != null) detailName.text = "";
        if (detailDesc != null) detailDesc.text = "Select an upgrade";
        if (buyButton != null) buyButton.interactable = false;
        if (buyButtonLabel != null) buyButtonLabel.text = "BUY";
    }

    private void HighlightTabs()
    {
        for (int i = 0; i < attackTabButtons.Length; i++)
        {
            if (i >= attacks.Count) continue;
            if (attackTabLabels != null && i < attackTabLabels.Length && attackTabLabels[i] != null)
                attackTabLabels[i].text = attacks[i].attackName;
            if (attackTabIcons != null && i < attackTabIcons.Length && attackTabIcons[i] != null)
                attackTabIcons[i].sprite = attacks[i].baseIcon;

            var colors = attackTabButtons[i].colors;
            colors.normalColor = (i == selectedAttack) ? Color.white : new Color(0.6f, 0.6f, 0.6f);
            attackTabButtons[i].colors = colors;
        }
    }
}