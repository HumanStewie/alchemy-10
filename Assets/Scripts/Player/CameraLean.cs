using UnityEngine;

public class CameraLean : MonoBehaviour
{
    [SerializeField] private float attackDamping = 0.5f;
    [SerializeField] private float decayDamping = 0.3f;
    [SerializeField] private float walkStrength = 0.075f;
    [SerializeField] private float slideStrength = 0.2f;
    [SerializeField] private float strengthResponse = 5f;
    
    private Vector3 _dampedAcceleration;
    private Vector3 _dampedAccelerationVelocity;
    private float _smoothedStrength;
    
    public void Initialize()
    {
        _smoothedStrength = walkStrength;
    }

    public void UpdateLean(float deltaTime, Vector3 acceleration, Vector3 up)
    {
        var planarAcceleration = Vector3.ProjectOnPlane(acceleration, up);
        var damping = planarAcceleration.magnitude > _dampedAcceleration.magnitude ? attackDamping : decayDamping;
       
        _dampedAcceleration = Vector3.SmoothDamp
        (
            current: _dampedAcceleration,
            target: planarAcceleration,
            currentVelocity: ref _dampedAccelerationVelocity,
            smoothTime: damping,
            maxSpeed: float.PositiveInfinity,
            deltaTime: deltaTime
        );
        
        //get the rotation axis based on the acceleration vector
        var leanAxis = Vector3.Cross(_dampedAcceleration.normalized, up).normalized;
        
        //reset rotation of that of its parent
        transform.localRotation = Quaternion.identity;
        
        //smoothly lerp between lean rotation
        var targetStrength = walkStrength;
        _smoothedStrength = Mathf.Lerp(_smoothedStrength, targetStrength, 1f - Mathf.Exp(-strengthResponse * deltaTime));
        
        //rotate around lean axis
        transform.rotation = Quaternion.AngleAxis(_dampedAcceleration.magnitude * _smoothedStrength, leanAxis) * transform.rotation;
    }
}
