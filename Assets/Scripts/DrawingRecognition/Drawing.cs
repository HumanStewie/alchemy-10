using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class Drawing : MonoBehaviour
{
    public static Drawing Instance;

    public LineRenderer lr;
    public List<Vector2> currentStroke = new();

    [Header("Jar References")]
    [SerializeField] private Collider labelCollider;
    [SerializeField] private Camera drawCamera;

    [Header("Settings")]
    [SerializeField] private float minDistanceBetweenPoints = 0.5f;
    [SerializeField] private float surfaceOffset = 0.005f;
    [SerializeField] private float standardUVScale = 500f;
    [SerializeField] private LayerMask drawLayerMask = ~0;

    private bool isDrawing = false;
    private Vector2 lastUvPoint;
    private Bounds localMeshBounds;
    private bool hasCachedBounds = false;

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

        // Ensure this GameObject follows the label's transform if separate
        if (labelCollider != null && transform.parent != labelCollider.transform)
        {
            transform.SetParent(labelCollider.transform, false);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            transform.localScale = Vector3.one;
        }

        if (drawCamera == null)
        {
            drawCamera = Camera.main;
        }

        CacheColliderBounds();
    }

    private void CacheColliderBounds()
    {
        if (labelCollider == null) return;

        if (labelCollider is MeshCollider mc && mc.sharedMesh != null)
        {
            localMeshBounds = mc.sharedMesh.bounds;
            hasCachedBounds = true;
        }
        else if (labelCollider is BoxCollider box)
        {
            localMeshBounds = new Bounds(box.center, box.size);
            hasCachedBounds = true;
        }
    }

    void Update()
    {
        if (BookMovement.Instance != null && (BookMovement.Instance.isIdle || BookMovement.Instance.isInanimation))
        {
            if (isDrawing)
            {
                // Cancel stroke cleanly if animation interrupts
                isDrawing = false;
                ClearVisuals();
            }
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = drawCamera.ScreenPointToRay(Input.mousePosition);
            if (TryGetLabelHit(ray, out RaycastHit hit))
            {
                ClearVisuals();
                isDrawing = true;
                AddHitPoint(hit);
            }
        }
        else if (Input.GetMouseButton(0) && isDrawing)
        {
            Ray ray = drawCamera.ScreenPointToRay(Input.mousePosition);
            if (TryGetLabelHit(ray, out RaycastHit hit))
            {
                Vector2 uv = CalculateUV(hit);
                Vector2 currentUv = new Vector2(uv.x * standardUVScale, uv.y * standardUVScale);

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

            if (GestureRecognizer.Instance != null)
            {
                GestureRecognizer.Instance.DoEverything(currentStroke);
            }
        }
    }

    private bool TryGetLabelHit(Ray ray, out RaycastHit hitResult)
    {
        RaycastHit[] hits = Physics.RaycastAll(ray, Mathf.Infinity, drawLayerMask, QueryTriggerInteraction.Collide);

        foreach (RaycastHit hit in hits)
        {
            if (hit.collider == labelCollider)
            {
                hitResult = hit;
                return true;
            }
        }

        hitResult = default;
        return false;
    }

    private Vector2 CalculateUV(RaycastHit hit)
    {
        if (hit.collider is MeshCollider mc && !mc.convex && hit.textureCoord != Vector2.zero)
        {
            return hit.textureCoord;
        }

        if (!hasCachedBounds)
        {
            CacheColliderBounds();
        }

        Vector3 localHit = hit.collider.transform.InverseTransformPoint(hit.point);
        Vector3 size = localMeshBounds.size;
        Vector3 min = localMeshBounds.min;
        Vector3 max = localMeshBounds.max;

        // Flatten along the thinnest axis
        if (size.z <= size.x && size.z <= size.y)
        {
            float u = Mathf.InverseLerp(min.x, max.x, localHit.x);
            float v = Mathf.InverseLerp(min.y, max.y, localHit.y);
            return new Vector2(u, v);
        }
        else if (size.x <= size.y && size.x <= size.z)
        {
            float u = Mathf.InverseLerp(min.z, max.z, localHit.z);
            float v = Mathf.InverseLerp(min.y, max.y, localHit.y);
            return new Vector2(u, v);
        }
        else
        {
            float u = Mathf.InverseLerp(min.x, max.x, localHit.x);
            float v = Mathf.InverseLerp(min.z, max.z, localHit.z);
            return new Vector2(u, v);
        }
    }

    private void AddHitPoint(RaycastHit hit)
    {
        Vector2 uv = CalculateUV(hit);
        Vector2 uvPoint = new Vector2(uv.x * standardUVScale, uv.y * standardUVScale);
        currentStroke.Add(uvPoint);
        lastUvPoint = uvPoint;

        Vector3 worldPointWithOffset = hit.point + (hit.normal * surfaceOffset);

        Vector3 localPos = labelCollider.transform.InverseTransformPoint(worldPointWithOffset);

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