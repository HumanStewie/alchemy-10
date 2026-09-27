using System;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UpgradeButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    public TextMeshProUGUI Upgradename;
    public TextMeshProUGUI description;

    public UpgradeBase upgradeData;
    [SerializeField] private float scaleAmount = 1.5f;
    [SerializeField] private float scaleClick = 0.9f;
    [SerializeField] private float sensitivity = 15f;
    private RectTransform rect;
    private Vector3 initialScale;
    private bool isHovered = false;
    private bool isDown = false;
    private void Start()
    {
        rect = GetComponent<RectTransform>();
        initialScale = rect.localScale;

        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    public void SetUpgrade(UpgradeBase data)
    {
        upgradeData = data;
        Upgradename.text = data.name;
        description.text = data.Description;
    }

    public void OnClick()
    {
        PlayerUpgradeManager.Instance.upgrades.Add(upgradeData);
    }

    private void Update()
    {
        if (isHovered)
        {
            rect.localScale = Vector3.Lerp(rect.localScale, Vector3.one * scaleAmount, 1f - Mathf.Exp(-sensitivity * Time.deltaTime));
        }
        else
        {
            rect.localScale = Vector3.Lerp(rect.localScale, initialScale, 1f - Mathf.Exp(-sensitivity * Time.deltaTime));
        }

        if (isDown)
            rect.localScale = Vector3.Lerp(rect.localScale, Vector3.one * scaleClick, 1f - Mathf.Exp(-sensitivity * Time.deltaTime));
        else
            rect.localScale = Vector3.Lerp(rect.localScale, initialScale, 1f - Mathf.Exp(-sensitivity * Time.deltaTime));
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovered = true;
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isDown = false;

    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isDown = true;

    }
}