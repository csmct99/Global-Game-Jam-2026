using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class PlayerControls : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("Movement speed in units per second.")]
    [SerializeField] private float _moveSpeed = 5f;

    [Tooltip("Rotation speed in degrees per second.")]
    [SerializeField] private float _rotationSpeed = 720f;

    [Header("Dependencies")]
    [Tooltip("The Rigidbody2D component used for movement. Must be set to Kinematic.")]
    [SerializeField] private Rigidbody2D _rigidbody;
    [SerializeField] private WeaponBase _weapon;

    private Camera _mainCamera;

    private InputAction _moveAction;
    private Vector2 _currentMoveInput;

    private InputAction _attackAction;
    private float _curAttackInput;

    private Vector2 _mousePosition;

    private void Awake()
    {
        InitializeDependencies();
    }

    private void Update()
    {
        ReadInput();
    }

    private void FixedUpdate()
    {
        // Physics-based movement should happen in FixedUpdate
        HandleMovement();
        HandleRotation();
    }

    private void InitializeDependencies()
    {
        _mainCamera = Camera.main;
        if (_mainCamera == null)
        {
            Debug.LogError($"{nameof(PlayerControls)}: Main Camera not found! Rotation will not work.");
        }

        if (_rigidbody == null)
        {
            // Try to get it automatically if forgot to assign in inspector
            _rigidbody = GetComponent<Rigidbody2D>();
            
            if (_rigidbody == null)
            {
                Debug.LogError($"{nameof(PlayerControls)}: Rigidbody2D reference is missing.");
            }
        }

        // Verify Rigidbody settings
        if (_rigidbody != null && _rigidbody.bodyType != RigidbodyType2D.Kinematic)
        {
            Debug.LogWarning($"{nameof(PlayerControls)}: Rigidbody2D should be set to 'Kinematic' for this controller.");
        }

        
        InputActionAsset actions = InputSystem.actions;
        if (actions != null)
        {
            _moveAction = actions.FindAction("Move");
            if (_moveAction == null)
            {
                Debug.LogError($"{nameof(PlayerControls)}: Could not find an Input Action named 'Move'.");
            }

            _attackAction = actions.FindAction("Attack");
            if (_attackAction == null)
            {
                Debug.LogError($"{nameof(PlayerControls)}: Could not find an Input Action named 'Fire'.");
            }
        }
        else
        {
            Debug.LogError($"{nameof(PlayerControls)}: PlayerInput component is missing.");
        }

        if(_weapon == null)
        {
            Debug.LogWarning($"{nameof(PlayerControls)}: Missing weapon.");
        }
    }

    private void ReadInput()
    {
        if (_moveAction != null)
        {
            _currentMoveInput = _moveAction.ReadValue<Vector2>();
        }

        if (_attackAction != null)
        {
            float oldAttackInput = _curAttackInput;
            float newAttackInput = _attackAction.ReadValue<float>();

            if(oldAttackInput != newAttackInput)
            {
                HandleAttackInput(oldAttackInput, newAttackInput);
            }
        }

        if (Mouse.current != null)
        {
            _mousePosition = Mouse.current.position.ReadValue();
        }
    }

    private void HandleMovement()
    {
        if (_rigidbody == null) return;

        // Calculate target position based on input
        Vector2 displacement = _currentMoveInput * _moveSpeed * Time.fixedDeltaTime;
        Vector2 targetPosition = _rigidbody.position + displacement;

        _rigidbody.MovePosition(targetPosition);
    }

    private void HandleRotation()
    {
        if (_rigidbody == null || _mainCamera == null) return;

        // Convert mouse screen position to world position
        Vector3 mouseWorldPos = _mainCamera.ScreenToWorldPoint(_mousePosition);
        mouseWorldPos.z = 0f; // Ensure z is flat for 2D

        Vector2 direction = (Vector3)mouseWorldPos - transform.position;

        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        
        targetAngle -= 90f;

        // Smoothly rotate towards the target angle
        Quaternion targetRotation = Quaternion.Euler(0, 0, targetAngle);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, _rotationSpeed * Time.fixedDeltaTime);
    }

    private void HandleAttackInput(float oldAttackInput, float newAttackInput) {
        if(oldAttackInput != newAttackInput && _weapon != null) {
            _curAttackInput = newAttackInput;
            _weapon.toggleFire(newAttackInput == 1);
        }
    }
}