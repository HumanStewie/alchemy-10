using UnityEngine;

public class Player : MonoBehaviour
{
    private PlayerActionInputs actionInputs;

    void Start()
    {
        actionInputs = new PlayerActionInputs();
        actionInputs.Enable();
    }

    void Update()
    {
        // Process these datas
        var input = actionInputs.Player;
        var deltaTime = Time.deltaTime;
        var cameraInput = input.Look.ReadValue<Vector2>();
        var movement = input.Move.ReadValue<Vector2>();
        var sprint = input.Sprint.IsPressed();
        var jump = input.Jump.WasPressedThisFrame();
        var crouch = input.Crouch.WasPressedThisFrame();

        // For interactions, same as above, check if its pressed, if it is, do smth
        var interact = input.Interact.IsPressed();
    }
}
