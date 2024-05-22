using UnityEngine;
using Unity.Cinemachine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5.0f;
    public float sprintSpeed = 10.0f;
    public float speedChangeRate = 10.0f;
    public float rotationSmoothTime = 0.12f;
    [Header("Jump Settings")]
    public bool grounded = true;
    public float groundCheckOffset = -0.15f;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    [Header("Camera Settings")]
    public GameObject freeLookCamera;
    public bool lockCameraPosition;
    public float cameraSensitivityX = 10;
    public bool invertX;
    public float cameraSensitivityY = 10;
    public bool invertY;

    [Header("Animator Setting")]
    public string speedParameter = "Speed";
    public string motionSpeedParameter = "MotionSpeed";
    
    [Header("Sound Settings")]
    public AudioClip[] footstepSounds;
    public AudioClip landingSound;

    // Player movement variables
    private float _speed;
    private float _animationBlend;
    private float _targetRotation = 0.0f;
    private float _rotationVelocity;
    private float _verticalVelocity;

    // Animator IDs
    private int _animIDSpeed;
    private int _animIDMotionSpeed;
    private int _animIDGrounded;

    private CinemachineOrbitalFollow _orbitalFollow;
    private Rigidbody _rigidbody;
    private Animator _animator;

    private void Start()
    {
        freeLookCamera.TryGetComponent(out _orbitalFollow);
        TryGetComponent(out _rigidbody);
        TryGetComponent(out _animator);

        AssignAnimationIDs();
    }

    private void Update()
    {
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
        GroundCheck();
        Move();
    }

    private void FixedUpdate()
    {
        
    }

    private void LateUpdate()
    {
        Look();
    }

    private void Look()
    {
        if(_orbitalFollow)
        {
            if (invertX)
            {
                _orbitalFollow.HorizontalAxis.Value -= InputManager.Instance.LookValue.x * cameraSensitivityX * Time.deltaTime;
            }
            else
            {
                _orbitalFollow.HorizontalAxis.Value += InputManager.Instance.LookValue.x * cameraSensitivityX * Time.deltaTime;
            }

            if (invertY)
            {
                _orbitalFollow.VerticalAxis.Value = Mathf.Clamp(_orbitalFollow.VerticalAxis.Value + InputManager.Instance.LookValue.y * cameraSensitivityY * Time.deltaTime, _orbitalFollow.VerticalAxis.Range.x, _orbitalFollow.VerticalAxis.Range.y);
                
            }
            else
            {
                _orbitalFollow.VerticalAxis.Value = Mathf.Clamp(_orbitalFollow.VerticalAxis.Value - InputManager.Instance.LookValue.y * cameraSensitivityY * Time.deltaTime, _orbitalFollow.VerticalAxis.Range.x, _orbitalFollow.VerticalAxis.Range.y);
            }
        }
    }

    public void Move()
    {
        if(_rigidbody)
        {
            float targetSpeed = InputManager.Instance.SprintValue ? sprintSpeed : moveSpeed;
            if(InputManager.Instance.MoveValue == Vector2.zero)
            {
                targetSpeed = 0;
            }

            float currentHorizontalSpeed = new Vector3(_rigidbody.linearVelocity.x, 0, _rigidbody.linearVelocity.z).magnitude;
            float speedOffset = 0.1f;

            if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
            {
                _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed, Time.deltaTime * speedChangeRate);
                _speed = Mathf.Round(_speed * 100f) / 100f;
            }
            else
            {
                _speed = targetSpeed;
            }

            _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * speedChangeRate);
            if (_animationBlend < 0.01f)
            {
                _animationBlend = 0;
            }

            Vector3 inputDirection = new Vector3(InputManager.Instance.MoveValue.x, 0, InputManager.Instance.MoveValue.y).normalized;

            if(InputManager.Instance.MoveValue != Vector2.zero)
            {
                _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + freeLookCamera.transform.eulerAngles.y;
                float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity, rotationSmoothTime);
                transform.rotation = Quaternion.Euler(0, rotation, 0);
            }

            Vector3 targetDirection = Quaternion.Euler(0, _targetRotation, 0) * Vector3.forward;


            _rigidbody.MovePosition(transform.position + targetDirection.normalized * (_speed * Time.deltaTime) + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);

            // Set Animator Parameters
            if(_animator)
            {
                _animator.SetFloat(_animIDSpeed, _animationBlend);
                _animator.SetFloat(_animIDMotionSpeed, 1.0f);
            }
        }
    }

    private void GroundCheck()
    {
        // Set sphere position
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - groundCheckOffset, transform.position.z);
        grounded = Physics.CheckSphere(spherePosition, groundCheckRadius, groundLayer, QueryTriggerInteraction.Ignore);

        if(_animator)
        {
            _animator.SetBool(_animIDGrounded, grounded);
        }
    }

    private void AssignAnimationIDs()
    {
        _animIDSpeed = Animator.StringToHash(speedParameter);
        _animIDMotionSpeed = Animator.StringToHash(motionSpeedParameter);
        _animIDGrounded = Animator.StringToHash("Grounded");
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

    // 对应Animator中的OnFootStep事件
    public void OnFootstep(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight > 0.5)
        {
            if (footstepSounds.Length > 0)
            {
                int randomIndex = Random.Range(0, footstepSounds.Length);
                AudioSource.PlayClipAtPoint(footstepSounds[randomIndex], transform.position);
            }
        }
    }

    // 对应Animator中的OnLand事件
    public void OnLand(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight > 0.5)
        {
            AudioSource.PlayClipAtPoint(landingSound, transform.position);
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Draw ground check sphere
        Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
        Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

        if(grounded) Gizmos.color = transparentGreen;
        else Gizmos.color = transparentRed;

        Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y - groundCheckOffset, transform.position.z), groundCheckRadius);
    }
}