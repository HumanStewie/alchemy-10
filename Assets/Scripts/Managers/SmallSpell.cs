using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SmallSpell : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public enum SpellNodeID { Node_1_1, Node_1_2, Node_2_1, Node_2_2 }

    [Header("Upgrade Setup")]
    public SpellNodeID nodeID;
    public UpgradeBase upgrades;
    public TMP_Text nameText;
    public TMP_Text desText;

    [Header("Visual Feedback Colors")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color hoverColor = new Color(1.25f, 1.25f, 1.25f, 1f); // Brighter
    [SerializeField] private Color lockedColor = new Color(0.35f, 0.35f, 0.35f, 0.8f); // Darker dimmed
    [SerializeField] private Color purchasedColor = new Color(0.6f, 0.9f, 0.6f, 1f);

    [HideInInspector] public bool isPurchased = false;
    [HideInInspector] public bool isUnlocked = false;

    private Button button;
    private Image buttonImage;
    private SpellBranchManager branchManager;

    private void Awake()
    {
        button = GetComponent<Button>();
        buttonImage = GetComponent<Image>();
        branchManager = GetComponentInParent<SpellBranchManager>();

        button.onClick.AddListener(OnClickUpgrade);
    }

    public void SetState(bool unlocked, bool purchased)
    {
        isUnlocked = unlocked;
        isPurchased = purchased;

        button.interactable = unlocked && !purchased;

        if (purchased)
        {
            buttonImage.color = purchasedColor;
        }
        else if (!unlocked)
        {
            buttonImage.color = lockedColor;
        }
        else
        {
            buttonImage.color = normalColor;
        }
    }

    private void OnClickUpgrade()
    {
        if (!isUnlocked || isPurchased) return;

        if (upgrades != null)
        {
            FindFirstObjectByType<AttackTabWheel>().fullUpg.Add(upgrades);
            upgrades.Upgrade();
        }

        if (branchManager != null)
        {
            branchManager.OnNodePurchased(nodeID);
        }

        if (MusicManager.Instance != null)
        {
            MusicManager.Instance.PlayUpgradeButtonSound();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (upgrades != null)
        {
            if (nameText != null) nameText.text = upgrades.name;
        }
        if (upgrades != null)
        {
            if (desText != null) desText.text = upgrades.Description;
        }

        transform.DOScale(3.5f, 0.15f).SetUpdate(true);

        if (isUnlocked && !isPurchased)
        {
            buttonImage.DOColor(hoverColor, 0.15f).SetUpdate(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.DOScale(3.0f, 0.15f).SetUpdate(true);

        if (isPurchased)
        {
            buttonImage.DOColor(purchasedColor, 0.15f).SetUpdate(true);
        }
        else if (!isUnlocked)
        {
            buttonImage.DOColor(lockedColor, 0.15f).SetUpdate(true);
        }
        else
        {
            buttonImage.DOColor(normalColor, 0.15f).SetUpdate(true);
        }
    }
}