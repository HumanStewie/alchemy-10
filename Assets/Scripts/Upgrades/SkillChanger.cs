using NUnit.Framework;
using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Analytics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillChanger : MonoBehaviour
{
    public List<GameObject> Buttons = new();
    public List<String> buttonDescriptions = new();
    public float radius = 200f;

    public GameObject pivot;

    public List<GameObject> ChildrenUpgrades = new();

    public int currentCount = 0;


    public GameObject textDes;

    public Button Next;
    void Start()
    {
        if (Buttons.Count == 0) return;

        float angleStep = 360f / Buttons.Count;

        for (int i = 0; i < Buttons.Count; i++)
        {
            int index = i;
            GameObject btn = Buttons[i];
            RectTransform rect = btn.GetComponent<RectTransform>();

            float angle = (i * angleStep) + 90f;
            float x = Mathf.Cos(angle * Mathf.Deg2Rad) * radius;
            float y = Mathf.Sin(angle * Mathf.Deg2Rad) * radius;

            rect.anchoredPosition = new Vector2(x, y);

            EventTrigger trigger = btn.AddComponent<EventTrigger>();

            EventTrigger.Entry enterEntry = new EventTrigger.Entry();
            enterEntry.eventID = EventTriggerType.PointerEnter;
            enterEntry.callback.AddListener((data) => { ShowDescription(index); });
            trigger.triggers.Add(enterEntry);


            EventTrigger.Entry exitEntry = new EventTrigger.Entry();
            exitEntry.eventID = EventTriggerType.PointerExit;
            exitEntry.callback.AddListener((data) => { ClearDescription(); });
            trigger.triggers.Add(exitEntry);


            Next.AddComponent<EventTrigger>();
            Next.onClick.AddListener(GoToNextWave);
        }
    }

    void GoToNextWave()
    {
        this.gameObject.SetActive(false);
        GameManager.Instance.GoNextWave();
    }
    void ShowDescription(int index)
    {
        if (index < buttonDescriptions.Count && textDes != null)
        {
            TMP_Text tmpText = textDes.GetComponent<TMP_Text>();
            if (tmpText != null) tmpText.text = buttonDescriptions[index];
        }
    }
    private void ClearDescription()
    {
        if (textDes != null)
        {
            TMP_Text tmpText = textDes.GetComponent<TMP_Text>();
            if (tmpText != null) tmpText.text = "";
        }
    }
    public void Pressing(Button clicked)
    {
        if (Buttons.IndexOf(clicked.gameObject) < currentCount) {
            transform.Rotate(0, -72, 0);
            currentCount--;
            if (currentCount == -1)
            {
                currentCount = 4;
            }
            for (int i = 0; i < ChildrenUpgrades.Count; i++)
            {
                if (i ==  currentCount * 2 || i == currentCount * 2 + 1)
                {
                    ChildrenUpgrades[i].SetActive(true);
                }
                else
                {
                    ChildrenUpgrades[i].SetActive(false);
                }
            }
        }
        else if (Buttons.IndexOf(clicked.gameObject) > currentCount)
        {
            transform.Rotate(0, 72, 0);
            currentCount++;
        }
    }
}
