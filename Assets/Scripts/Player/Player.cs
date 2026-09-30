using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Needed Objects")]
    [SerializeField] private PlayerCharacter playerCharacter;
    [SerializeField] private PlayerCamera playerCamera;
    [SerializeField] private CameraSpring cameraSpring;
    [SerializeField] private CameraLean cameraLean;

    private PlayerActionInputs inputActions;
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        inputActions = new PlayerActionInputs();
        inputActions.Enable();
        playerCharacter.Initialize();
        playerCamera.Initialize(playerCharacter.GetCameraTarget());
        // playerCamera.Initialize(playerCharacter.GetCameraTarget());
        
        cameraSpring.Initialize();
        cameraLean.Initialize();
    }

    private void Update()
    {
        var input = inputActions.Player;
        var deltaTime = Time.deltaTime;
        // Same idea as below, this is how we read look data
        var cameraInput = new CameraInput { Look = input.Look.ReadValue<Vector2>() };
        //if (BookMovement.Instance != null && BookMovement.Instance.isIdle)
        //{
            playerCamera.UpdateRotation(cameraInput, playerCharacter.GetCameraTarget());
        //}

        Vector2 moveVector = input.Move.ReadValue<Vector2>();

        // Get character input and update it. This is mainly how we will take InputAction and use them
        // Essentially, we "queue" the input, preparing to throw into UpdateInput.
        var characterInput = new CharacterInput
        {
            Rotation = playerCamera.transform.rotation,
            Movement = moveVector,
            Sprint = input.Sprint.IsPressed(),
            Jump = input.Jump.WasPressedThisFrame(),
            JumpSustain = input.Jump.IsPressed(),
            // Crouch = input.Crouch.IsPressed() ? CrouchInput.Hold : CrouchInput.Release
            Crouch = input.Crouch.WasPressedThisFrame(),
            Interact = input.Interact.WasPressedThisFrame(),
        };
        playerCharacter.UpdateInput(characterInput);
        playerCharacter.UpdateBody(deltaTime);

        bool isMoving = moveVector.sqrMagnitude > 0.01f ||
                        Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) ||
                        Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.W);

        if (MusicManager.Instance != null)
        {
            MusicManager.Instance.SetWalkingSound(isMoving);
        }
    }

    private void LateUpdate()
    {
        var deltaTime = Time.deltaTime;
        // playerCamera.UpdatePosition(playerCharacter.GetCameraTarget());
        playerCamera.UpdatePosition(playerCharacter.GetCameraTarget());
        cameraSpring.UpdateSpring(deltaTime, playerCharacter.GetCameraTarget().up);
        cameraLean.UpdateLean(deltaTime, playerCharacter.GetCharacterData().acceleration, playerCharacter.GetCameraTarget().up);
    }

    private void OnDisable()
    {
        if (MusicManager.Instance != null)
        {
            MusicManager.Instance.SetWalkingSound(false);
        }
    }

    private void OnDestroy()
    {
        inputActions.Dispose();
    }
}