using Runtime;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControls : MonoBehaviour
{
	#region Private Fields


	private Camera _mainCamera;
	private InputAction _moveAction;
	private Vector2 _currentMoveInput;
	private Vector2 _mousePosition;
    private InputAction _attackAction;
    private float _curAttackInput;

    private float _moveSpeed;
    private float _rotationSpeed;
    private Rigidbody2D _rigidbody;
    private Agent _agent;
    

	#endregion

	#region MonoBehaviour Methods

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

	#endregion


	#region Public Methods

    public void Initialize(float moveSpeed, float rotationSpeed, Rigidbody2D rigidbody, Agent agent)
    {
        _moveSpeed = moveSpeed;
        _rotationSpeed = rotationSpeed;
        _rigidbody = rigidbody;
        _agent = agent;
        
        InitializeDependencies();
    }

	#endregion

	#region Private Methods

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
				Debug.LogError($"{nameof(PlayerControls)}: Could not find an Input Action named 'Attack'.");
			}
		}
		else
		{
			Debug.LogError($"{nameof(PlayerControls)}: PlayerInput component is missing.");
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
                _agent.ToggleWeaponFire(newAttackInput == 1);
                _curAttackInput = newAttackInput;
            }
        }

		if (Mouse.current != null)
		{
			_mousePosition = Mouse.current.position.ReadValue();
		}
	}

	private void HandleMovement()
	{
		if (_rigidbody == null)
			return;

		// Calculate target position based on input
		Vector2 displacement = _currentMoveInput * _moveSpeed * Time.fixedDeltaTime;
		Vector2 targetPosition = _rigidbody.position + displacement;

		_rigidbody.MovePosition(targetPosition);
	}

	private void HandleRotation()
	{
		if (_rigidbody == null || _mainCamera == null)
			return;

		// Convert mouse screen position to world position
		Vector3 mouseWorldPos = _mainCamera.ScreenToWorldPoint(_mousePosition);
		mouseWorldPos.z = 0f; // Ensure z is flat for 2D

		Vector2 direction = (Vector3) mouseWorldPos - transform.position;

		float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

		targetAngle -= 90f;

		// Smoothly rotate towards the target angle
		Quaternion targetRotation = Quaternion.Euler(0, 0, targetAngle);
		transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, _rotationSpeed * Time.fixedDeltaTime);
	}

	#endregion
}
