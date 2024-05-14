using Unity.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private static InputManager _instance;

    public static InputManager Instance => _instance;

    private void Awake()
    {
        if (Instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        
    }

    public string moveActionName = "Move";
    [SerializeField, ReadOnly]
    private Vector2 moveValue;
    public Vector2 MoveValue => moveValue;

    public string jumpActionName = "Jump";
    [SerializeField, ReadOnly]
    private bool jumpValue;
    public bool JumpValue => jumpValue;

    public string lookActionName = "Look";
    [SerializeField, ReadOnly]
    private Vector2 lookValue;
    public Vector2 LookValue => lookValue;

    public string cancelActionName = "Cancel";
    [SerializeField, ReadOnly]
    private bool cancelValue;
    public bool CancelValue => cancelValue;

    private InputAction _moveAction;
    private InputAction _jumpAction;
    private InputAction _lookAction;
    private InputAction _cancelAction;
    
    void Start()
    {
        _moveAction = InputSystem.actions.FindAction(moveActionName);
        _jumpAction = InputSystem.actions.FindAction(jumpActionName);
        _lookAction = InputSystem.actions.FindAction(lookActionName);
        _cancelAction = InputSystem.actions.FindAction(cancelActionName);
    }

    void Update()
    {
        GetMoveValue();
        GetJumpValue();
        GetLookValue();
        GetCancelValue();
    }

    private void GetMoveValue()
    {
        moveValue = _moveAction.ReadValue<Vector2>();
    }

    private void GetJumpValue()
    {
        jumpValue = _jumpAction.ReadValue<float>() > 0;
    }

    private void GetLookValue()
    {
        lookValue = _lookAction.ReadValue<Vector2>();
    }

    private void GetCancelValue()
    {
        cancelValue = _cancelAction.ReadValue<float>() > 0;
    }

    public bool CancelButtonPressed()
    {
        return _cancelAction.WasPressedThisFrame();
    }
}
