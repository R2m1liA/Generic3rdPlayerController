using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public string moveActionName = "Move";
    public Vector2 moveValue;

    public string jumpActionName = "Jump";
    public bool jumpValue;

    private InputAction moveAction;
    private InputAction jumpAction;
    
    void Start()
    {
        moveAction = InputSystem.actions.FindAction(moveActionName);
        jumpAction = InputSystem.actions.FindAction(jumpActionName);
    }

    void Update()
    {
        GetMoveValue();
        GetJumpValue();
    }

    public void GetMoveValue()
    {
        moveValue = moveAction.ReadValue<Vector2>();
    }

    public void GetJumpValue()
    {
        jumpValue = jumpAction.ReadValue<float>() > 0;
    }
}
