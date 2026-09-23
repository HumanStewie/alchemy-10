using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


[RequireComponent(typeof(LineRenderer))]
public class Drawing : MonoBehaviour, IPointerUpHandler, IPointerDownHandler, IDragHandler
{
    public LineRenderer lr;
    public List<Vector2> currentStroke = new List<Vector2>();

    [Header("Settings")]
    [SerializeField] private float minDistanceBetweenPoints = 10f; 
    [SerializeField] private float lineZPlane = 5f;
    public void OnDrag(PointerEventData eventData)
    {
        Vector2 lastPos = currentStroke[currentStroke.Count - 1];
        if (Vector2.Distance(lastPos, eventData.position) >= minDistanceBetweenPoints)
        {
            AddPoint(eventData.position, eventData.pressEventCamera);
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
    }

    void Start()
    {
        lr = GetComponent<LineRenderer>();
        lr.positionCount = 0;
    }

    void AddPoint(Vector2 screenPoint, Camera cam)
    {
        if (!IsInsideBox(screenPoint, cam)) return;

        currentStroke.Add(screenPoint);

        int index = currentStroke.Count - 1;
        lr.positionCount = currentStroke.Count;

        Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(screenPoint.x, screenPoint.y, lineZPlane));
        lr.SetPosition(index, worldPos);
    }

    public void ClearVisuals()
    {
        currentStroke.Clear();
        lr.positionCount = 0;
    }

    private bool IsInsideBox(Vector2 screenPoint, Camera eventCamera)
    {
        return RectTransformUtility.RectangleContainsScreenPoint(GetComponent<RectTransform>(), screenPoint, eventCamera);
    }
}
