using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class Drawing : MonoBehaviour
{
    public static Drawing Instance;

    public LineRenderer lr;
    public List<Vector2> currentStroke = new List<Vector2>();

    [Header("Jar References")]
    [SerializeField] private Collider labelCollider;
    [SerializeField] private Camera drawCamera;

    [Header("Settings")]
    [SerializeField] private float minDistanceBetweenPoints = 0.5f;
    [SerializeField] private float surfaceOffset = 0.005f;
    [SerializeField] private float standardUVScale = 500f;

    private bool isDrawing = false;
    private Vector2 lastUvPoint;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        lr = GetComponent<LineRenderer>();
        lr.startWidth = 0.003f;
        lr.endWidth = 0.003f;
        lr.useWorldSpace = false;
        lr.positionCount = 0;

        if (drawCamera == null)
        {
            drawCamera = Camera.main;
        }
    }

    void Update()
    {

        if (BookMovement.Instance != null && (BookMovement.Instance.isIdle || BookMovement.Instance.isInanimation))
        {
            return;
        }
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = drawCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit) && hit.collider == labelCollider)
            {
                ClearVisuals();
                isDrawing = true;
                AddHitPoint(hit);
            }
        }
        else if (Input.GetMouseButton(0) && isDrawing)
        {
            Ray ray = drawCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit) && hit.collider == labelCollider)
            {
                Vector2 currentUv = new Vector2(hit.textureCoord.x * standardUVScale, hit.textureCoord.y * standardUVScale);
                if (Vector2.Distance(lastUvPoint, currentUv) >= minDistanceBetweenPoints)
                {
                    AddHitPoint(hit);
                }
            }
        }
        else if (Input.GetMouseButtonUp(0) && isDrawing)
        {
            isDrawing = false;

            if (currentStroke.Count < 5)
            {
                ClearVisuals();
                return;
            }

            GestureRecognizer.Instance.DoEverything(currentStroke);
        }
    }

    private void AddHitPoint(RaycastHit hit)
    {
        Vector2 uvPoint = new Vector2(hit.textureCoord.x * standardUVScale, hit.textureCoord.y * standardUVScale);
        currentStroke.Add(uvPoint);
        lastUvPoint = uvPoint;

        Vector3 worldPointWithOffset = hit.point + (hit.normal * surfaceOffset);
        Vector3 localPos = transform.InverseTransformPoint(worldPointWithOffset);

        lr.positionCount = currentStroke.Count;
        lr.SetPosition(currentStroke.Count - 1, localPos);
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