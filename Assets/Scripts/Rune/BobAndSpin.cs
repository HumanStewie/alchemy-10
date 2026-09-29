using UnityEngine;

public class BobAndSpin : MonoBehaviour
{
    public Vector3 rotationSpeed = new Vector3(0f, 90f, 0f);
    public float bobHeight = 0.5f;

    public float bobFrequency = 2f;

    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        transform.Rotate(rotationSpeed * Time.deltaTime, Space.World);

        float newY = startPosition.y + Mathf.Sin(Time.time * bobFrequency) * bobHeight;
        transform.position = new Vector3(startPosition.x, newY, startPosition.z);
    }
}