using UnityEngine;

public struct CameraInput
{
    public Vector2 Look;
}

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private float cameraSensitivity = 0.2f;
    private Vector3 lastTargetUp = Vector3.up;

    public void Initialize(Transform cameraTarget)
    {
        transform.position = cameraTarget.position;
        transform.rotation = cameraTarget.rotation;
        lastTargetUp = cameraTarget.up;
    }

    public void UpdateRotation(CameraInput input, Transform cameraTarget)
    {
        Vector3 targetUp = cameraTarget.up;

        // When the surface normal changes (e.g. walking onto curved surfaces, walls, or ceilings),
        // rotate the camera by the delta to keep it aligned with the surface
        if (lastTargetUp != Vector3.zero && lastTargetUp != targetUp)
        {
            Quaternion surfaceDelta = Quaternion.FromToRotation(lastTargetUp, targetUp);
            transform.rotation = surfaceDelta * transform.rotation;
        }
        lastTargetUp = targetUp;

        // Yaw: rotate around the surface normal (cameraTarget.up)
        float yawDelta = input.Look.x * cameraSensitivity;
        transform.rotation = Quaternion.AngleAxis(yawDelta, targetUp) * transform.rotation;

        // Pitch: rotate around the camera's local right axis, clamped relative to the surface normal
        float pitchDelta = -input.Look.y * cameraSensitivity;
        float currentAngleToUp = Vector3.Angle(transform.forward, targetUp);
        float targetAngleToUp = Mathf.Clamp(currentAngleToUp + pitchDelta, 1f, 179f);
        float clampedPitchDelta = targetAngleToUp - currentAngleToUp;
        transform.rotation = Quaternion.AngleAxis(clampedPitchDelta, transform.right) * transform.rotation;

        // Ensure zero roll relative to targetUp
        transform.rotation = Quaternion.LookRotation(transform.forward, targetUp);
    }

    public void UpdatePosition(Transform cameraTarget)
    {
        transform.position = cameraTarget.position;
    }
    
}
