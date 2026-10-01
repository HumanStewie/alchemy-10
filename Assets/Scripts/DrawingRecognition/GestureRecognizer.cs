using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GestureRecognizer : MonoBehaviour
{
    public static GestureRecognizer Instance;

    private float totLength = 0;
    public float spaceInterval = 0;
    public float standardSize = 500f;

    public List<Vector2> listForChecking = new List<Vector2>();

    private float maxX = 0;
    private float maxY = 0;
    private float minX = 0;
    private float minY = 0;

    [Header("Template Recording")]
    public bool isGettingTemp = false;
    public SpellTemplate temp;

    [Header("Templates & Thresholds")]
    public List<SpellTemplate> templates = new List<SpellTemplate>();
    [SerializeField] private float maxAllowedError = 50f;

    [Header("Equipped Spell & Cooldowns")]
    public SpellTemplate preparedSpell;
    private Dictionary<string, float> spellCooldowns = new Dictionary<string, float>();

    [SerializeField] TMP_Text spellText;

    [System.Serializable]
    public struct SpellMaterialPair
    {
        public SpellTemplate spell;
        public Material jamMaterial;
    }

    [Header("Jam Materials Mapping")]
    [SerializeField] private List<SpellMaterialPair> spellJamMaterials = new List<SpellMaterialPair>();
    [SerializeField] private Material defaultJamMaterial;

    private void Awake()
    {
        Instance = this;
        for (int i = 0; i < templates.Count; i++)
        {
            if (templates[i] != null)
            {
                templates[i] = Instantiate(templates[i]);
            }
        }
    }

    private void Start()
    {
        if (preparedSpell != null)
        {
            ApplyJamMaterialForSpell(preparedSpell);
        }
        else if (defaultJamMaterial != null && BookMovement.Instance != null)
        {
            BookMovement.Instance.SetJamMaterial(defaultJamMaterial);
        }
    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.P))
        //{
           // isGettingTemp = !isGettingTemp;
            //Debug.Log($"Record Mode: {isGettingTemp}");
        //}

        UpdateSpellHUDText();

        if (BookMovement.Instance != null && BookMovement.Instance.isIdle && !BookMovement.Instance.isInanimation && !BookMovement.Instance.isDisabled)
        {
            if (Input.GetMouseButtonDown(0))
            {
                TryCastSpell();
            }
        }
        if (BookMovement.Instance.isDisabled)
        {
            spellText.text = "Jam is stuck, get away from the PanCake!";
        }
        else
        {
            spellText.text = "Spell is available";
        }
    }

    private void UpdateSpellHUDText()
    {
        if (spellText == null || preparedSpell == null) return;

        if (spellCooldowns.TryGetValue(preparedSpell.spellName, out float readyTime))
        {
            float remaining = readyTime - Time.time;
            if (remaining > 0f)
            {
                spellText.text = $"{preparedSpell.spellName} (CD: {remaining:F1}s)";
                return;
            }
        }

        spellText.text = $"{preparedSpell.spellName} [READY]";
    }

    private void TryCastSpell()
    {
        if (preparedSpell == null) return;

        if (spellCooldowns.TryGetValue(preparedSpell.spellName, out float spellReadyTime))
        {
            if (Time.time < spellReadyTime)
            {
                float remainingTime = spellReadyTime - Time.time;
                Debug.Log($"{preparedSpell.spellName} cooldown active! {remainingTime:F1}s remaining.");
                if (spellText != null)
                {
                    spellText.text = $"{preparedSpell.spellName} on Cooldown: {remainingTime:F1}s";
                }
                return;
            }
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Vector3 targetPoint = ray.origin + ray.direction * 30f;
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            targetPoint = hit.point;
        }

        preparedSpell.Cast(gameObject, targetPoint, 1f);

        spellCooldowns[preparedSpell.spellName] = Time.time + preparedSpell.cooldown;
    }

    private void ApplyJamMaterialForSpell(SpellTemplate spell)
    {
        if (BookMovement.Instance == null || spell == null) return;

        Material selectedMat = defaultJamMaterial;

        for (int i = 0; i < spellJamMaterials.Count; i++)
        {
            if (spellJamMaterials[i].spell != null &&
               (spellJamMaterials[i].spell == spell || spellJamMaterials[i].spell.spellName == spell.spellName))
            {
                selectedMat = spellJamMaterials[i].jamMaterial;
                break;
            }
        }

        if (selectedMat != null)
        {
            BookMovement.Instance.SetJamMaterial(selectedMat);
        }
    }

    public void DoEverything(List<Vector2> points)
    {
        findTotalLength(points);
    }

    void findTotalLength(List<Vector2> points)
    {
        totLength = 0;
        for (int i = 1; i < points.Count; i++)
        {
            totLength += Vector2.Distance(points[i - 1], points[i]);
        }
        spaceInterval = totLength / 63f;
        NewList(points);
    }

    public void NewList(List<Vector2> points)
    {
        listForChecking.Clear();

        float accumulatedDist = 0f;
        List<Vector2> working = new List<Vector2>(points);
        listForChecking.Add(working[0]);

        for (int i = 1; i < working.Count; i++)
        {
            float segmentDist = Vector2.Distance(working[i - 1], working[i]);

            if (accumulatedDist + segmentDist >= spaceInterval)
            {
                float t = (spaceInterval - accumulatedDist) / segmentDist;
                Vector2 newPoint = Vector2.Lerp(working[i - 1], working[i], t);

                listForChecking.Add(newPoint);
                working.Insert(i, newPoint);
                accumulatedDist = 0f;
            }
            else
            {
                accumulatedDist += segmentDist;
            }
        }

        while (listForChecking.Count < 64)
        {
            listForChecking.Add(working[working.Count - 1]);
        }

        checkMaxMin(listForChecking);
    }

    public void checkMaxMin(List<Vector2> points)
    {
        minX = points[0].x;
        maxX = points[0].x;
        minY = points[0].y;
        maxY = points[0].y;

        for (int i = 1; i < points.Count; ++i)
        {
            if (points[i].x > maxX) maxX = points[i].x;
            if (points[i].x < minX) minX = points[i].x;
            if (points[i].y > maxY) maxY = points[i].y;
            if (points[i].y < minY) minY = points[i].y;
        }

        Scaler(points);
    }

    public void Scaler(List<Vector2> points)
    {
        float width = Mathf.Max(maxX - minX, 0.001f);
        float height = Mathf.Max(maxY - minY, 0.001f);

        float ratioX = standardSize / width;
        float ratioY = standardSize / height;

        for (int i = 0; i < points.Count; ++i)
        {
            points[i] = new Vector2(points[i].x * ratioX, points[i].y * ratioY);
        }

        Centroidizer(points);
    }

    public void Centroidizer(List<Vector2> points)
    {
        float totX = 0;
        float totY = 0;

        for (int i = 0; i < points.Count; ++i)
        {
            totX += points[i].x;
            totY += points[i].y;
        }

        Vector2 centroid = new Vector2(totX / points.Count, totY / points.Count);

        for (int i = 0; i < points.Count; ++i)
        {
            points[i] = points[i] - centroid;
        }

        checkTemplate(points);
    }

    public void checkTemplate(List<Vector2> points)
    {
        if (isGettingTemp)
        {
            Drawing.Instance.SaveAsTemplate(temp, points);
            BookMovement.Instance.ReturnToIdle();
            Drawing.Instance.ClearVisuals();
            return;
        }

        SpellTemplate bestTemp = null;
        float lowestDistance = float.MaxValue;

        for (int t = 0; t < templates.Count; t++)
        {
            SpellTemplate current = templates[t];
            if (current == null || current.points == null || current.points.Count != points.Count) continue;

            float totalDist = 0f;
            for (int i = 0; i < points.Count; i++)
            {
                totalDist += Vector2.Distance(points[i], current.points[i]);
            }
            float avgDist = totalDist / points.Count;

            if (avgDist < lowestDistance)
            {
                lowestDistance = avgDist;
                bestTemp = current;
            }
        }

        BookMovement.Instance.ReturnToIdle();
        Drawing.Instance.ClearVisuals();

        if (lowestDistance <= maxAllowedError && bestTemp != null)
        {
            bool wasHoldingSword = BookMovement.Instance != null && BookMovement.Instance.IsHoldingSword;
            bool isNewSpellNonSword = !(bestTemp is SwordSpell);

            if (wasHoldingSword && isNewSpellNonSword)
            {
                BookMovement.Instance.JamSwordSwing(SwordState.Stop, null);

                if (preparedSpell is SwordSpell prevSword)
                {
                    prevSword.ResetSwordState();
                }

                if (templates.Find(s => s is SwordSpell) is SwordSpell templateSword)
                {
                    templateSword.ResetSwordState();
                }
            }

            preparedSpell = bestTemp;
            ApplyJamMaterialForSpell(preparedSpell);

            if (spellCooldowns.TryGetValue(preparedSpell.spellName, out float readyTime) && Time.time < readyTime)
            {
                float remaining = readyTime - Time.time;
                spellText.text = $"Current Spell: {preparedSpell.spellName} (CD: {remaining:F1}s)";
            }
            else
            {
                spellText.text = $"Current Spell: {preparedSpell.spellName}";
            }

            if (MusicManager.Instance != null && Camera.main != null)
            {
                MusicManager.Instance.PlaySpellRecognizedSound(Camera.main.transform.position);
            }
        }
        else
        {
            Debug.Log($"Failed to recognize gesture. Closest was: {bestTemp?.spellName} ({lowestDistance:F1})");
            spellText.text = $"Failed to recognize gesture. Closest was: {bestTemp?.spellName}";
        }
    }
}