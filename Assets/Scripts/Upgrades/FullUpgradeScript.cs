using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class AttackTabWheel : MonoBehaviour
{
    [System.Serializable]
    public class TabConfig
    {
        public string tabName;
        public Button tabButton;            
        public GameObject upgradeContainer;
        public float offsetOfObject;
        [Tooltip("The exact Z-rotation the wheel should have when this tab is selected")]
        public float tabAngle;              
    }

    [Header("Wheel Reference")]
    [SerializeField] private RectTransform wheelTransform;
    [SerializeField] private float rotateDuration = 0.4f;

    [Header("Keep Upright Settings")]
    [Tooltip("If checked, keeps the tabs upright automatically as the wheel spins.")]
    [SerializeField] private bool keepTabsUpright = true;

    [Header("Input Settings")]
    [Tooltip("Enable cycling through tabs using W / S or Arrow keys.")]
    [SerializeField] private bool enableKeyboardCycling = true;

    [Header("Tab Visual Settings")]
    [Tooltip("Color tint of the currently selected tab button.")]
    [SerializeField] private Color selectedColor = Color.white;
    [Tooltip("Color tint of unselected tab buttons.")]
    [SerializeField] private Color unselectedColor = new Color(0.5f, 0.5f, 0.5f, 1f);

    [Header("5 Tabs Configuration")]
    [SerializeField] private List<TabConfig> tabs = new();

    private Dictionary<Button, GameObject> tabDictionary = new();
    private Tween spinTween;
    private int currentSelectedIndex = 0;

    public List<UpgradeBase> fullUpg = new();

    private void Awake()
    {
        for (int i = 0; i < tabs.Count; i++)
        {
            TabConfig tab = tabs[i];
            if (tab == null || tab.tabButton == null) continue;

            tabDictionary[tab.tabButton] = tab.upgradeContainer;

            int index = i;
            tab.tabButton.onClick.AddListener(() => SelectTab(index));
        }
    }

    private void OnEnable()
    {
        if (tabs != null && tabs.Count > 0 && currentSelectedIndex >= 0 && currentSelectedIndex < tabs.Count)
        {
            UpdateTabVisuals(currentSelectedIndex);
        }
    }

    private void Start()
    {
        if (tabs.Count > 0)
        {
            SelectTab(0);
        }
    }

    private void Update()
    {
        if (!enableKeyboardCycling || tabs == null || tabs.Count == 0) return;

        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            CycleTab(-1);
        }
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            CycleTab(1);
        }
    }

    public void CycleTab(int direction)
    {
        if (tabs == null || tabs.Count == 0) return;

        int startIndex = (currentSelectedIndex >= 0 && currentSelectedIndex < tabs.Count) ? currentSelectedIndex : 0;
        int newIndex = (startIndex + direction) % tabs.Count;
        if (newIndex < 0) newIndex += tabs.Count;

        SelectTab(newIndex);
    }

    private void LateUpdate()
    {
        if (!keepTabsUpright || wheelTransform == null) return;

        float currentWheelZ = wheelTransform.localEulerAngles.z;
        Vector3 counterRotation = new Vector3(0, 0, -currentWheelZ);

        for (int i = 0; i < tabs.Count; i++)
        {
            if (tabs[i] != null && tabs[i].tabButton != null)
            {
                tabs[i].tabButton.transform.localEulerAngles = counterRotation - new Vector3(0, 0, tabs[i].offsetOfObject);
            }
        }
    }

    public void SelectTab(int index)
    {
        if (index < 0 || index >= tabs.Count) return;

        currentSelectedIndex = index;
        TabConfig selectedTab = tabs[index];

        float currentAngle = wheelTransform.localEulerAngles.z;
        float delta = Mathf.DeltaAngle(currentAngle, selectedTab.tabAngle);
        float targetAngle = currentAngle + delta;

        spinTween?.Kill();
        spinTween = wheelTransform.DOLocalRotate(new Vector3(0, 0, targetAngle), rotateDuration, RotateMode.FastBeyond360)
            .SetEase(Ease.OutBack)
            .SetUpdate(true);

        // 2. Hide all other tab containers
        foreach (var pair in tabDictionary)
        {
            if (pair.Value != null)
            {
                pair.Value.SetActive(false);
            }
        }

        // 3. Show the selected tab's container
        if (selectedTab.upgradeContainer != null)
        {
            selectedTab.upgradeContainer.SetActive(true);
        }

        // 4. Update tab button visuals (grey out unselected tabs)
        UpdateTabVisuals(index);
    }

    private void UpdateTabVisuals(int selectedIndex)
    {
        for (int i = 0; i < tabs.Count; i++)
        {
            TabConfig tab = tabs[i];
            if (tab == null || tab.tabButton == null) continue;

            bool isSelected = (i == selectedIndex);
            Color targetColor = isSelected ? selectedColor : unselectedColor;

            if (tab.tabButton.transition == Selectable.Transition.ColorTint)
            {
                ColorBlock colors = tab.tabButton.colors;
                colors.normalColor = targetColor;
                colors.selectedColor = targetColor;
                colors.highlightedColor = isSelected ? selectedColor : Color.Lerp(unselectedColor, selectedColor, 0.35f);
                colors.pressedColor = targetColor * 0.8f;
                tab.tabButton.colors = colors;
            }
            else if (tab.tabButton.targetGraphic != null)
            {
                tab.tabButton.targetGraphic.color = targetColor;
            }
        }
    }
}