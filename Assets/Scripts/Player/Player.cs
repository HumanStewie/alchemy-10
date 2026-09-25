using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Needed Objects")]
    [SerializeField] private PlayerCharacter playerCharacter;
    [SerializeField] private PlayerCamera playerCamera;
    

    private PlayerActionInputs inputActions;
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        inputActions = new PlayerActionInputs();
        inputActions.Enable();
        playerCharacter.Initialize();
        playerCamera.Initialize(playerCharacter.GetCameraTarget());
        // playerCamera.Initialize(playerCharacter.GetCameraTarget());
        
    }

    private void Update()
    {
        var input = inputActions.Player;
        var deltaTime = Time.deltaTime;
        // Same idea as below, this is how we read look data
        var cameraInput = new CameraInput { Look = input.Look.ReadValue<Vector2>() };
        if (BookMovement.Instance.isIdle)
        {
            playerCamera.UpdateRotation(cameraInput);
        }

        // Get character input and update it. This is mainly how we will take InputAction and use them
        // Essentially, we "queue" the input, preparing to throw into UpdateInput.
        var characterInput = new CharacterInput
        {
            Rotation = playerCamera.transform.rotation,
            Movement = input.Move.ReadValue<Vector2>(),
            Sprint = input.Sprint.IsPressed(),
            Jump = input.Jump.WasPressedThisFrame(),
            JumpSustain = input.Jump.IsPressed(),
            // Crouch = input.Crouch.IsPressed() ? CrouchInput.Hold : CrouchInput.Release
            Crouch = input.Crouch.WasPressedThisFrame()
        };
        playerCharacter.UpdateInput(characterInput);
        playerCharacter.UpdateBody(deltaTime);
        

    }

    private void LateUpdate()
    {
        // playerCamera.UpdatePosition(playerCharacter.GetCameraTarget());
        playerCamera.UpdatePosition(playerCharacter.GetCameraTarget());
    }

    private void OnDestroy()
    {
        inputActions.Dispose();
    }
}
