using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class GestureRecognizer : MonoBehaviour
{
    public static GestureRecognizer Instance;

    float totLength = 0;
    public float spaceInterval = 0;
    public float boxSize = 500;

    public List<Vector2> listForChecking;


    public float maxX = 0;
    public float maxY = 0;

    public float minX = -999;
    public float minY = -999;


    public float standardSize = 500;

    public bool isGettingTemp;


    [Header("Templates & Thresholds")]
    public List<SpellTemplate> templates = new List<SpellTemplate>();
    [SerializeField] private float maxAllowedError = 50f;

    public SpellTemplate temp;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (!isGettingTemp)
            {
                isGettingTemp = true;
            }
            else
            {
                isGettingTemp = false;
            }
        }
    }
    public void DoEverything(List<Vector2> points)
    {
        findTotalLength(points);
    }

    void findTotalLength(List<Vector2> points)
    {
        totLength = 0;
        spaceInterval = 0;
        for (int i = 1; i < points.Count; i++)
        {
            totLength += Vector2.Distance(points[i - 1], points[i]);
        }
        spaceInterval = totLength / 63;
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

        for (int i = 0; i < points.Count; ++i)
        {
            if (points[i].x >= maxX)
            {
                maxX = points[i].x;
            }
            if (points[i].y >= maxY)
            {
                maxY = points[i].y;
            }
        }

        for (int i = 0; i < points.Count; ++i)
        {
            if (points[i].x <= minX)
            {
                minX = points[i].x;
            }
            if (points[i].y <= minY)
            {
                minY = points[i].y;
            }
        }

        Scaler(points);
    }

    public void Scaler(List<Vector2> points)
    {
        float width = maxX - minX;  
        float height = maxY - minY;
        
        float ratioX = standardSize / width;
        float ratioY = standardSize / height;


        for (int i = 0; i < points.Count; ++i)
        {
            points[i] = new Vector2(points[i].x * ratioX, points[i].y * ratioY);
        }
        Centroidizer(points);
    }

    public void Centroidizer(List<Vector2> points) {

        float totX = 0;
        float totY = 0;


        for (int i = 0; i < points.Count; ++i)
        {
            totX += points[i].x;    
            totY += points[i].y;
        }

        Vector2 centroid = new Vector2(totX/points.Count, totY/points.Count);


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
        }


        SpellTemplate bestTemp = null;
        float lowestDistance = float.MaxValue;

        for (int t = 0; t < templates.Count; t++)
        {
            SpellTemplate current = templates[t];
            if (current.points == null || current.points.Count != points.Count) continue;

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

        if (lowestDistance <= maxAllowedError && bestTemp != null)
        {
            SpellExecutor.Instance.ExecuteSpell(bestTemp);
        }

    }
}

