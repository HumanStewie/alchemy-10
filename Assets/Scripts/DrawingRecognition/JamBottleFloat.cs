using UnityEngine;

public class JamBottleFloat : MonoBehaviour
{
    [Header("Bobbing Settings")]
    [SerializeField] private float bobSpeed = 2.5f;    
    [SerializeField] private float bobHeight = 0.05f; 

    [Header("Spinning Settings")]
    [SerializeField] private float spinSpeed = 40f;     
    [SerializeField] private Vector3 spinAxis = Vector3.up;

    private float currentY;

    private Vector3 initialLocalPos;

    public bool canSpin = true;

    void Start()
    {
        initialLocalPos = transform.localPosition;
        currentY = -0.06f;
    }

    void Update()
    {
        if (GetComponent<BookMovement>().isIdle) {
            Spinning();
        }
    }


    void Spinning()
    {
        float newY = initialLocalPos.y + (Mathf.Sin(Time.time * bobSpeed) * bobHeight);

        currentY += spinSpeed * Time.deltaTime;

        transform.localRotation = Quaternion.Euler(0, currentY, 0f);
    }
}