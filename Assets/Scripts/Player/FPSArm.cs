using UnityEngine;

public class FPSArm : MonoBehaviour
{
    [SerializeField] private Transform target;

    [SerializeField] private Vector3 offset;
    void Start()
    {
        transform.position = target.position + offset;
        transform.rotation = target.rotation;
    }
}
