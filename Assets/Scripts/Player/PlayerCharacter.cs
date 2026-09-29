using KinematicCharacterController;
using Unity.VisualScripting;
using UnityEngine;

public struct CharacterInput
{
    public Quaternion Rotation;
    public Vector2 Movement;
    public bool Sprint;
    public bool Jump;
    public bool JumpSustain;
    public bool Crouch;
    public bool Interact;
}
public struct Character
{
    public CharacterState state;
    public Stance stance;
    public Vector3 velocity;
    public Vector3 acceleration;
}
public enum CharacterState
{
    Grounded,
    InAir,
    WallRun
}

public enum Stance
{
    Stand, Slide
}

public class PlayerCharacter : MonoBehaviour, ICharacterController
{
    public Character GetCharacterData() => character;

    [Header("Dependencies")]
    [SerializeField] private KinematicCharacterMotor motor;
    [SerializeField] private Transform meshTarget;
    public Transform GetMeshTarget() => meshTarget;
    [SerializeField] private    Wallrunning wallRun;
    [SerializeField] private Transform cameraTarget;
    public Transform GetCameraTarget() => cameraTarget;
    [SerializeField] private Animator armsAnimator;
    public Animator ArmsAnimator => armsAnimator;
    [SerializeField] private Animator jarAnimator;
    public Animator JarAnimator => jarAnimator;

    [Header("Move")]
    [SerializeField] public float walkSpeed = 10.0f;
    [SerializeField] private float sprintResponse = 15f;
    [SerializeField] private float walkResponse = 25f;
    [SerializeField] private float wallWalkResponse = 25f;

    [Header("Jump")]
    [SerializeField] private float jumpStrength = 20f;
    [SerializeField] private float gravity = -90f;
    [SerializeField] private float coyoteTime = 0.2f;
    [SerializeField] private float jumpBufferTime = 0.2f;
    [SerializeField] private float airAcceleration = 70f;
    [SerializeField] private float maxFallSpeed = 30f;
    [SerializeField] private float jumpEndEarlyFallSpeed = 10f;
    [SerializeField] private float maxAirSpeedWhenJumpedFromStandStill = 12f;


    private Quaternion requestedRotation;
    private Vector3 requestedMovement;
    private bool requestedSprint;
    private bool requestedJump;
    private bool requestedJumpSustain;
    private bool requestedCrouch;

    private float timeSinceUngrounded;
    private float timeSinceJumpRequest;

    private Character character;
    private Character lastFrameCharacter;

    private Vector3 velocityBeforeJump;
    private Vector3 movementForce;
    private bool wasUngroundedByJump;

    public void Initialize()
    {
        motor.CharacterController = this;
        character.stance = Stance.Stand;
        character.state = CharacterState.Grounded;
    }
    public void UpdateInput(CharacterInput input)
    {
        requestedRotation = input.Rotation;
        
        // Get requested 2D input from character input, given from InputActions in Player.cs
        requestedMovement = new Vector3(input.Movement.x, 0, input.Movement.y);
        // Normalize it so our diagonal movement dont explode
        requestedMovement = Vector3.ClampMagnitude(requestedMovement, 1.0f);
        // Relative movement to camera, this affects the pitch, causing requestedMovement to point *upward* (flying), fix below    
        // This also cause the angle of movement vector to be closer to flat, which causes buggy diagonals when pitch is up and down
        Vector3 planarForward = Vector3.ProjectOnPlane(requestedRotation *  Vector3.forward, motor.CharacterUp).normalized;
        if (planarForward.sqrMagnitude < 0.001f)
        {
            
        }

        Quaternion surfaceYaw = Quaternion.LookRotation(planarForward, motor.CharacterUp);
        requestedMovement = surfaceYaw * requestedMovement;
        
        requestedSprint = input.Sprint;

        var wasRequestedJump = requestedJump;
        requestedJump = requestedJump ||  input.Jump;
        if (requestedJump && !wasRequestedJump)
            timeSinceJumpRequest = 0;
        requestedJumpSustain = input.JumpSustain;

        requestedCrouch = input.Crouch;

        if (input.Interact)
        {
            BookMovement.Instance.ToggleBookState();
            armsAnimator.SetBool("IsDrawing", !BookMovement.Instance.isIdle);
            jarAnimator.SetBool("IsDrawing", !BookMovement.Instance.isIdle);
            if (MusicManager.Instance != null)
            {
                MusicManager.Instance.PlayHandDrawModeSound(transform.position);
            }

        }
    }
    public void UpdateBody(float deltaTime)
    {
        
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, (transform.position + character.velocity));
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + movementForce);
        Gizmos.color = Color.green;
        Gizmos.DrawLine(Vector3.zero, motor.GroundingStatus.GroundNormal);
    }

    public void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
    {
        currentRotation = Quaternion.LookRotation(
            Vector3.ProjectOnPlane(requestedRotation * Vector3.forward, motor.CharacterUp),
            motor.CharacterUp
        );
            if (motor.GroundingStatus.FoundAnyGround && motor.GroundingStatus.GroundCollider.gameObject.layer == LayerMask.NameToLayer("CurvedFloor"))
            {
                currentRotation = Quaternion.Lerp(
                    currentRotation,
                    Quaternion.FromToRotation(motor.CharacterUp, motor.GroundingStatus.GroundNormal) * currentRotation, 
                    1.0f - Mathf.Exp(-wallWalkResponse * deltaTime)
                );
            }
        
    }

    public void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
    {
        armsAnimator.SetBool("IsGroundedMoving", false);
        jarAnimator.SetBool("IsGroundedMoving", false);

        character.acceleration = Vector3.zero;

        // State checking
        // Check air or ground
        character.state = motor.GroundingStatus.IsStableOnGround ? CharacterState.Grounded : CharacterState.InAir;
        
        // If we are in air and a jump is requested
        if (character.state is CharacterState.InAir)
        {
            // Manage state (also checks requested jump, really odd choice, probably need better handling
            wallRun.ManageState(ref requestedJump, character, ref motor, ref currentVelocity, deltaTime);
            
            // If we are already wall running, just switch to wall run state
            if (wallRun.IsWallRunning)
            {
                character.state = CharacterState.WallRun;
            }
        } 
        // Otherwise, we stop wall run. This is hard coded, the moment player land on the ground, stop all wall run
        else if (character.state is CharacterState.Grounded)
        {
            wallRun.StopWallRun();
        }
        
        // If we are in wall run, and there's no more wall or we stopped wall running, stop everything
        if (character.state is CharacterState.WallRun)
        {
            if (!wallRun.IsWallRunning || !wallRun.CheckWall(ref motor))
            {
                wallRun.StopWallRun();
                character.state = CharacterState.InAir;
            }
        }
        // State handling
        switch (character.state)
        {
            case CharacterState.Grounded:
                GroundMovement(ref currentVelocity, deltaTime);
                break;
            case CharacterState.InAir:
                InAirMovement(ref currentVelocity, deltaTime);
                break;
            case CharacterState.WallRun:
                WallRunMovement(ref currentVelocity, deltaTime);
                break;
        }
        
        character.velocity = currentVelocity;
    }

    private void GroundMovement(ref Vector3 currentVelocity, float deltaTime)
    {
        timeSinceUngrounded = 0;
        character.state = CharacterState.Grounded;
        wasUngroundedByJump = false;
        var groundedMovement = motor.GetDirectionTangentToSurface(
            direction: requestedMovement,
            surfaceNormal: motor.GroundingStatus.GroundNormal
        ) * requestedMovement.magnitude;
        
        if (groundedMovement.sqrMagnitude > 0)
        {
            armsAnimator.SetBool("IsGroundedMoving", true);
            jarAnimator.SetBool("IsGroundedMoving", true);
        }
        if (character.stance is Stance.Stand) {
            // Walking & Sprinting
            var speed = requestedSprint ? walkSpeed * 2 : walkSpeed;
            var response = requestedSprint ? sprintResponse : walkResponse;
            var moveVelocity = Vector3.Lerp(
                a: currentVelocity,
                b: groundedMovement * speed,
                t: 1.0f - Mathf.Exp(-response * deltaTime)
            );
            
            character.acceleration = moveVelocity - currentVelocity;
            currentVelocity = moveVelocity;
        }
        velocityBeforeJump = currentVelocity;
        if (requestedJump) Jump(ref currentVelocity, deltaTime);
        
    }

    private void InAirMovement(ref Vector3 currentVelocity, float deltaTime)
    {
        timeSinceUngrounded += deltaTime;
        character.state = CharacterState.InAir;
        // If movement in air
        if (requestedMovement.sqrMagnitude > 0)
        {
            // the *steer*. It makes our requested movement *slowly* pan to left and right
            movementForce = deltaTime * airAcceleration * requestedMovement;
            var airVelocity = Vector3.ProjectOnPlane(currentVelocity, motor.CharacterUp);
            movementForce += airVelocity;

            movementForce = velocityBeforeJump.magnitude > 0.5f 
                ? Vector3.ClampMagnitude(movementForce, velocityBeforeJump.magnitude) 
                : Vector3.ClampMagnitude(movementForce, maxAirSpeedWhenJumpedFromStandStill);
            currentVelocity += movementForce - airVelocity;
        }
            
        // If released jump butten and still going up (so not in apex)
        var fallSpeed = !requestedJumpSustain && currentVelocity.y > 0 ? gravity * jumpEndEarlyFallSpeed : gravity;
        // Constant gravity
        currentVelocity += fallSpeed * deltaTime * motor.CharacterUp;
        if (currentVelocity.y < -maxFallSpeed)
            currentVelocity.y = -maxFallSpeed;

        if (requestedJump)
        {
            
            Jump(ref currentVelocity, deltaTime);
        }
    }

    private void WallRunMovement(ref Vector3 currentVelocity, float deltaTime)
    {
        currentVelocity = Vector3.Lerp(
            a: currentVelocity,
            b: wallRun.UpdateWallRunVelocity(ref motor, deltaTime, velocityBeforeJump.magnitude),
            t: 1.0f - Mathf.Exp(-sprintResponse * deltaTime)
        );
        
        if (requestedJump)
        {
            character.state = CharacterState.InAir;     
            requestedJump = false;
            requestedCrouch = false;
            motor.ForceUnground();
            currentVelocity += wallRun.WallJump(ref motor);
            velocityBeforeJump = currentVelocity;
            wallRun.StopWallRun();
        }
    }

    private void Jump(ref Vector3 currentVelocity, float deltaTime)
    {
        if (character.state is CharacterState.Grounded || (!wasUngroundedByJump && timeSinceUngrounded < coyoteTime))
        {
            
            requestedJump = false;
            requestedCrouch = false;
            wasUngroundedByJump = true;
            motor.ForceUnground();
            // This snippet essentially allows us to jump in the air like we canceled gravity then jump, instead of stopping falling
            // We first get dot product between our velocity and up vector of character to get vertical speed
            var currentVerticalSpeed = Vector3.Dot(currentVelocity, motor.CharacterUp);
            // Then we find the max between them
            var targetVerticalSpeed = Mathf.Max(currentVerticalSpeed, jumpStrength);
            // target - current 
            currentVelocity += (targetVerticalSpeed - currentVerticalSpeed) * motor.CharacterUp;
        }
        else
        {
            timeSinceJumpRequest += deltaTime;
                
            requestedJump = timeSinceJumpRequest < jumpBufferTime;
        }
    }
    public void AfterCharacterUpdate(float deltaTime)
    {
        lastFrameCharacter = character;
    }

    public void BeforeCharacterUpdate(float deltaTime)
    {
    }

    public bool IsColliderValidForCollisions(Collider coll)
    {
        return true;
    }

    public void OnDiscreteCollisionDetected(Collider hitCollider)
    {
    }

    public void OnGroundHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
    {
    }

    public void OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
    {
    }

    public void PostGroundingUpdate(float deltaTime)
    {
    }

    public void ProcessHitStabilityReport(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, Vector3 atCharacterPosition, Quaternion atCharacterRotation, ref HitStabilityReport hitStabilityReport)
    {
    }


    public void SetHandVisibility(bool visible)
    {
        if (armsAnimator != null)
        {
            armsAnimator.SetBool("IsDrawing", !visible);
        }
        if (jarAnimator != null)
        {
            jarAnimator.SetBool("IsDrawing", !visible);
        }
    }

}
