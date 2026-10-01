using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TextShake : MonoBehaviour
{   
    public float rotate;
    private float bobAmount;
    private float bobSpeed;
    private float rotAmount;
    private float rotSpeed;

    private RectTransform frame;
    private Vector2 startPosition;
    private Vector2 startRotation;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        bobAmount = 10f;
        bobSpeed = Random.Range(2f, 4f);
        rotAmount = 3f;
        rotSpeed = Random.Range(1f, 3f);

        frame = GetComponent<RectTransform>();
        startPosition = frame.anchoredPosition;
    }

    // Update is called once per frame
    void Update()
    {
        // Apparently, a lot of games just use math instead of manually animating, so sin function
        float bob = Mathf.Sin(Time.time * bobSpeed) * bobAmount; 
        frame.anchoredPosition = startPosition + new Vector2(0, bob);
        float rotation = Mathf.Sin(Time.time * rotSpeed) * rotAmount;
        frame.localEulerAngles = new Vector3(0, 0, rotation) + new Vector3(0, 0, rotate);
    }
}
