using UnityEngine;

public struct CameraInput
{
    public Vector2 Look;
}

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private float cameraSensitivity = 0.2f;
    private Vector3 eulerAngles;
    public void Initialize(Transform cameraTarget)
    {
        transform.position = cameraTarget.position;
        transform.rotation = cameraTarget.rotation;
        
        transform.eulerAngles = eulerAngles = cameraTarget.eulerAngles;
    }

    public void UpdateRotation(CameraInput input, Transform cameraTarget)
    {
        eulerAngles += new Vector3(-input.Look.y, input.Look.x, 0) * cameraSensitivity;
        eulerAngles.x = Mathf.Clamp(eulerAngles.x, -89, 89);

        transform.rotation = Quaternion.Euler(eulerAngles);
    }

    public void UpdatePosition(Transform cameraTarget)
    {
        transform.position = cameraTarget.position;
    }
    
}
