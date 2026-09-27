using UnityEngine;
using KinematicCharacterController;


public class Wallrunning : MonoBehaviour
{
    [Header("Wall Run Settings")]
    [SerializeField] private float wallRunTime = 2.0f;
    [SerializeField] private float wallRunFallSpeed = -60.0f; // apply this similar to gravity
    [SerializeField] private float lookAngle = 30.0f;
    [SerializeField] private float initialUpwardBoost = 10f;
    [SerializeField] private float wallJumpVerticalStrength = 50f;
    [SerializeField] private float wallJumpHorizontalStrength = 50f;
    [SerializeField] private float exitWallTime;
    [SerializeField] private float stickStrength = 10f;

    [Header("Detect Settings")] 
    [SerializeField] private float wallCheckDistance = 1.0f;
    [SerializeField] private float wallCheckRadius = 0.2f;
    [SerializeField] private float castOffset = 1f;    

    private bool wallLeft;
    private bool wallRight;
    private RaycastHit rightWallHit;
    private RaycastHit leftWallHit;
    public bool IsWallRunning { get; private set; }
    public bool ExitingWall { get; set; }
    
    public float ExitWallTimer { get; set; }
    private RaycastHit currentHit;
    private Vector3 currentHorizontalVelocity;
    private Vector3 currentVerticalVelocity;
    
    // Check if currently there's any wall in range to begin wall running
    public bool CheckWall(ref KinematicCharacterMotor motor)
    {
        wallRight = Physics.SphereCast(transform.position + motor.CharacterUp * castOffset, wallCheckRadius, motor.CharacterRight, out rightWallHit, wallCheckDistance);
        wallLeft = Physics.SphereCast(transform.position + motor.CharacterUp * castOffset, wallCheckRadius, -motor.CharacterRight, out leftWallHit, wallCheckDistance);
        
        return wallLeft || wallRight;
    }
    
    // Snaps player to wall
    public void StartWallRun(ref KinematicCharacterMotor motor, ref Vector3 currentVelocity)
    {
        currentHit = wallLeft ? leftWallHit : rightWallHit;
        // Vector3 hitPositionToTeleport = Physics.ClosestPoint(transform.position, currentHit.collider, currentHit.collider.transform.position, currentHit.collider.transform.rotation);
        // Vector3 hitPositionToTeleport = (currentHit.point - motor.CharacterUp * castOffset) + (currentHit.normal * motor.Capsule.radius);
        // motor.SetPosition(hitPositionToTeleport);
        currentVerticalVelocity = motor.CharacterUp * initialUpwardBoost;
        
        IsWallRunning = true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(currentHit.point, 0.2f);
    }
    
    /// <summary>
    /// Get normalized direction to move, speed will be handled in PlayerCharacter
    /// </summary>
    /// <param name="motor"></param>
    /// <returns>Normalized direction to move</returns>
    private Vector3 GetWallRunDirection(ref KinematicCharacterMotor motor)
    {
        currentHit = wallLeft ? leftWallHit : rightWallHit;
        // var wallRunDirection = Vector3.Cross(currentWall.normal, motor.CharacterUp).normalized * speedBeforeWallRun;
        // if ((motor.CharacterForward - wallRunDirection).magnitude > (motor.CharacterForward - -wallRunDirection).magnitude)
        //     wallRunDirection = -wallRunDirection;
        var angleBetween = Vector3.Angle(currentHit.normal, motor.CharacterForward);
        // If we are not looking in around an angle to the normal, we should just fall off
        if (angleBetween < lookAngle && !CheckWall(ref motor))
        {
            IsWallRunning = false;
            return motor.CharacterForward;
        }
        var wallRunDirection = Vector3.ProjectOnPlane(motor.CharacterForward, currentHit.normal).normalized;
        return wallRunDirection;
    }

    public Vector3 UpdateWallRunVelocity(ref KinematicCharacterMotor motor, float deltaTime, float speedBeforeWallRun)
    {
        currentHit = wallLeft ? leftWallHit : rightWallHit;
        currentHorizontalVelocity = GetWallRunDirection(ref motor);
        currentVerticalVelocity += deltaTime * wallRunFallSpeed * motor.CharacterUp;
        var stickForce = stickStrength * -currentHit.normal;
        Vector3 trueVelocity = currentHorizontalVelocity * speedBeforeWallRun + currentVerticalVelocity;
        return Vector3.ProjectOnPlane(trueVelocity, currentHit.normal) + stickForce;
    }

    public Vector3 WallJump(ref KinematicCharacterMotor motor)
    {
        Debug.Log("WallJumping");

        ExitingWall = true;
        ExitWallTimer = exitWallTime;
        var trueJumpVelocity = wallJumpHorizontalStrength * currentHit.normal +
                               wallJumpVerticalStrength * motor.CharacterUp;
        return trueJumpVelocity;
    }

    public void StopWallRun()
    {
        IsWallRunning = false;
    }

    public void ManageState(ref bool requestedJump, Character character, ref KinematicCharacterMotor motor, ref Vector3 currentVelocity, float deltaTime)
    {
        // If we get a valid wall to run, we begin running
        if (requestedJump && CheckWall(ref motor) && !IsWallRunning && !ExitingWall)
        {
            requestedJump = false;
            character.state = CharacterState.WallRun;
            StartWallRun(ref motor, ref currentVelocity);
        }
        else if (ExitingWall)
        {
            if (IsWallRunning) StopWallRun();
            if (ExitWallTimer > 0) ExitWallTimer -= deltaTime;
            if (ExitWallTimer <= 0) ExitingWall = false;
        }
    }
}
