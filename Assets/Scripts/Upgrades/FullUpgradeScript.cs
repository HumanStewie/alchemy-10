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

    [Header("5 Tabs Configuration")]
    [SerializeField] private List<TabConfig> tabs = new();

    private Dictionary<Button, GameObject> tabDictionary = new();
    private Tween spinTween;


    public List<UpgradeBase> fullUpg = new();

    private void Awake()
    {
        for (int i = 0; i < tabs.Count; i++)
        {
            TabConfig tab = tabs[i];
            if (tab.tabButton == null) continue;

            tabDictionary[tab.tabButton] = tab.upgradeContainer;

            int index = i;
            tab.tabButton.onClick.AddListener(() => SelectTab(index));

           
        }
    }

    private void Start()
    {
        if (tabs.Count > 0)
        {
            SelectTab(0);
        }
    }

    private void LateUpdate()
    {
        if (!keepTabsUpright || wheelTransform == null) return;

        float currentWheelZ = wheelTransform.localEulerAngles.z;
        Vector3 counterRotation = new Vector3(0, 0, -currentWheelZ);

        for (int i = 0; i < tabs.Count; i++)
        {
            if (tabs[i].tabButton != null)
            {
                tabs[i].tabButton.transform.localEulerAngles = counterRotation - new Vector3(0,0, tabs[i].offsetOfObject);
            }
        }
    }

    public void SelectTab(int index)
    {
        if (index < 0 || index >= tabs.Count) return;

        TabConfig selectedTab = tabs[index];
        float targetAngle = selectedTab.tabAngle;

        spinTween?.Kill();
        spinTween = wheelTransform.DOLocalRotate(new Vector3(0, 0, targetAngle), rotateDuration)
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
    }
}