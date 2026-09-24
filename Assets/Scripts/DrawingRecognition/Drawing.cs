using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.EventSystems;


[RequireComponent(typeof(LineRenderer))]
public class Drawing : MonoBehaviour, IPointerUpHandler, IPointerDownHandler, IDragHandler
{
    public static Drawing Instance;

    public LineRenderer lr;
    public List<Vector2> currentStroke = new List<Vector2>();



    [Header("Settings")]
    [SerializeField] private float minDistanceBetweenPoints = 10f; 
    [SerializeField] private float lineZPlane = 5f;



    private void Awake()
    {
        Instance = this;
    }
    public void OnDrag(PointerEventData eventData)
    {
        RectTransform rect = GetComponent<RectTransform>();

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(rect, eventData.position, eventData.pressEventCamera, out Vector2 localPoint))
        {
            Vector2 lastPos = currentStroke[currentStroke.Count - 1];
            if (Vector2.Distance(lastPos, localPoint) >= minDistanceBetweenPoints)
            {
                AddPoint(eventData.position, eventData.pressEventCamera);
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        ClearVisuals();

        AddPoint(eventData.position, eventData.pressEventCamera);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (currentStroke.Count < 5)
        {
            ClearVisuals();
            return;
        }
        GestureRecognizer.Instance.DoEverything(currentStroke);
    }

    void Start()
    {
        lr = GetComponent<LineRenderer>();
        lr.startWidth = 0.01f;
        lr.endWidth = 0.01f;

        lr.useWorldSpace = false;
        lr.positionCount = 0;
    }

    void AddPoint(Vector2 screenPoint, Camera cam)
    {
        RectTransform rect = GetComponent<RectTransform>();

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(rect, screenPoint, cam, out Vector2 localPoint))
        {
            if (!rect.rect.Contains(localPoint)) return;

            currentStroke.Add(localPoint);

            lr.positionCount = currentStroke.Count;
            int index = currentStroke.Count - 1;

            Vector3 localPos = new Vector3(localPoint.x, localPoint.y, -0.002f);
            lr.SetPosition(index, localPos);
        }
    }

    public void ClearVisuals()
    {
        currentStroke.Clear();
        lr.positionCount = 0;
    }


    public void SaveAsTemplate(SpellTemplate targetAsset, List<Vector2> normalizedPoints)
    {
        targetAsset.points = new List<Vector2>(normalizedPoints);
#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(targetAsset);
        UnityEditor.AssetDatabase.SaveAssets();
#endif
        Debug.Log($"Saved {normalizedPoints.Count} points to {targetAsset.spellName}");
    }
}
