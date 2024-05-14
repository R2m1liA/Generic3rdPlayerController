using UnityEngine;
using Unity.Cinemachine;

public class PlayerController : MonoBehaviour
{
    public GameObject freeLookCamera;

    public float cameraSensitivityX = 10;
    public bool revertX;
    public float cameraSensitivityY = 10;
    public bool revertY;
    
    private CinemachineOrbitalFollow _orbitalFollow;

    private void Start()
    {
        freeLookCamera.TryGetComponent(out _orbitalFollow);
    }

    private void Update()
    {
        if (_orbitalFollow)
        {
            Look();
        }

        if (InputManager.Instance.CancelButtonPressed())
        {
            if (Cursor.lockState == CursorLockMode.Locked)
            {
                UnlockCursor();
            }
            else
            {
                LockCursor();
            }
        }
    }

    private void Look()
    {
        if (revertX)
        {
            _orbitalFollow.HorizontalAxis.Value -= InputManager.Instance.LookValue.x * cameraSensitivityX * Time.deltaTime;
        }
        else
        {
            _orbitalFollow.HorizontalAxis.Value += InputManager.Instance.LookValue.x * cameraSensitivityX * Time.deltaTime;
        }

        if (revertY)
        {
            _orbitalFollow.VerticalAxis.Value += InputManager.Instance.LookValue.y * cameraSensitivityY * Time.deltaTime;
        }
        else
        {
            _orbitalFollow.VerticalAxis.Value -= InputManager.Instance.LookValue.y * cameraSensitivityY * Time.deltaTime;
        }
    }

    public void Move()
    {
        
    }
    
    public void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    
    public void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}