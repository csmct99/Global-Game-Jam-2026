using System;

using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace Runtime
{
	public class MaskController : MonoBehaviour
	{
		#region Private Fields

		private InputAction _throwMaskInput;
		private IPossessable _currentPossessedTarget;

		[Header("References")]
		[SerializeField]
		private Rigidbody2D _rigidbody2D;

		[SerializeField]
		private Collider2D _collider;

		[SerializeField]
		private ParentConstraint _parentConstraint;

		[Header("Possession")]
		[SerializeField]
		private float _spawnDistanceFromTarget = 2f;

		[SerializeField]
		private float _possessStunDuration = 1.5f;

		[Header("Throws")]
		[SerializeField]
		private int _maxThrowAttempts = 2;

		[Tooltip("How far the mask travels during a throw.")]
		[SerializeField]
		private float _throwDistance = 8f;

		[Tooltip("How long it takes to travel the Throw Distance.")]
		[SerializeField]
		private float _throwDuration = 0.6f;

		private int _currentThrowsRemaining;
		private float _currentDeceleration; // Calculated per throw

		private float _recoveryEnterTime;

		[Header("Recovery")]
		[SerializeField]
		private float _maxRecoveryTime = 2f;

		[Header("Events")]
		[SerializeField]
		private UnityEvent _onPossessionBegin;

		[SerializeField]
		private UnityEvent _onPossessionEnd;

		[SerializeField]
		private UnityEvent _onInputsLocked;

		[SerializeField]
		private UnityEvent _onInputsUnlocked;
		
		[Header("SoundFX")]
		[SerializeField]
		private AudioClip enemyHurt;
		[SerializeField]
		private AudioClip[] enemyScreams;
		[SerializeField]
		private AudioClip enemyDeath;
		public GameObject audioInstance;

		private float _inputLockTimestamp;
		private bool _isInputLocked = false;

		private Vector2 _throwDirection;

		#endregion

		#region MonoBehaviour Methods

		private void Awake()
		{
			RefreshThrows();
			SetupInputs();
		}

		private void Update()
		{
			if (_throwMaskInput.WasPressedThisFrame())
			{
				if (_currentPossessedTarget != null)
				{
					DePossess();
				}
				else
				{
					ThrowMaskAtCursor();
				}
			}

			// Check if input needs unlocking
			if (_isInputLocked && Time.time - _inputLockTimestamp > _possessStunDuration)
			{
				UnlockInputs();
			}
		}

		private void FixedUpdate()
		{
			DampVelocity();
		}

		private void OnCollisionEnter2D(Collision2D other)
		{
			if (_currentPossessedTarget != null)
			{
				Debug.LogWarning("Got collision event while already possessing a target!");
				return;
			}

			if (TryGetPossessable(other.gameObject, out IPossessable possessable))
			{
				Possess(possessable);
			}
		}

		#endregion

		#region Private Methods

		private void DampVelocity()
		{
			// Standdown during possession
			if (_currentPossessedTarget != null)
				return;

			// Linear deceleration to create a "sliding" friction feel
			if (_rigidbody2D.linearVelocity.sqrMagnitude > 0.001f)
			{
				float currentSpeed = _rigidbody2D.linearVelocity.magnitude;
				float newSpeed = currentSpeed - (_currentDeceleration * Time.fixedDeltaTime);

				if (newSpeed < 0)
					newSpeed = 0;

				_rigidbody2D.linearVelocity = _rigidbody2D.linearVelocity.normalized * newSpeed;
			}
			else
			{
				_rigidbody2D.linearVelocity = Vector2.zero;
			}
		}

		private void LockInputs()
		{
			InputActionAsset actions = InputSystem.actions;
			actions.Disable();

			_isInputLocked = true;
			_inputLockTimestamp = Time.time;

			_onInputsLocked?.Invoke();
		}

		private void UnlockInputs()
		{
			InputActionAsset actions = InputSystem.actions;
			actions.Enable();

			_isInputLocked = false;

			_onInputsUnlocked?.Invoke();
		}

		private void ThrowMaskAtCursor()
		{
			if (_currentThrowsRemaining <= 0)
			{
				Debug.Log("Out of throws!");
				return;
			}

			_currentThrowsRemaining--;

			Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
			_throwDirection = (mouseWorldPos - (Vector2) transform.position).normalized;

			float initialSpeed = (2 * _throwDistance) / _throwDuration;

			// Deceleration required to reach 0 speed in exactly _throwDuration
			_currentDeceleration = initialSpeed / _throwDuration;

			_rigidbody2D.linearVelocity = _throwDirection * initialSpeed;
		}

		private void RefreshThrows()
		{
			_currentThrowsRemaining = _maxThrowAttempts;
		}

		private void SetupInputs()
		{
			InputActionAsset actions = InputSystem.actions;
			_throwMaskInput = actions.FindAction("ThrowMask");
		}

		private bool TryGetPossessable(GameObject target, out IPossessable possessable)
		{
			possessable = target.GetComponent<IPossessable>();
			return possessable != null;
		}

		private void Possess(IPossessable target)
		{
			if (_currentPossessedTarget != null)
			{
				throw new Exception("Tried to possess when we already have a possession active!");
			}

			target.BeginPossess(this);
			_currentPossessedTarget = target;

			// Disable mask collider and physics while possessed
			SetCollisionState(false);

			// Move the mask to be on top of the possessed target
			transform.position = target.GetGameObject().transform.position;
			_parentConstraint.AddSource(new ConstraintSource()
			{
				sourceTransform = target.GetGameObject().transform,
				weight = 1f
			});
			_parentConstraint.constraintActive = true;

			_onPossessionBegin?.Invoke();

			RefreshThrows();

			//Play Random Scream Sound
			audioInstance = SoundFXManager.instance.PlayRandomSoundFXClip(enemyScreams, transform, 1f);
			// Lock inputs for stun
			LockInputs();
		}

		private void DePossess()
		{
			if (_currentPossessedTarget == null)
			{
				throw new Exception("Tried to de-possess even though we are not currently possessing anything!");
			}

			if (audioInstance != null) Destroy(audioInstance);
			
			_currentPossessedTarget.StopPossess(this);

			// Stop following the possessed target
			_parentConstraint.RemoveSource(0);
			_parentConstraint.constraintActive = false;

			// Kill the target
			// TODO: Make this more interesting than a "delete"
			Destroy(_currentPossessedTarget.GetGameObject());
			// Drop cached reference
			_currentPossessedTarget = null;

			// Allow collisions again
			SetCollisionState(true);

			_onPossessionEnd?.Invoke();

			// Give throws back
			RefreshThrows();
			

			UnlockInputs();
			
			ThrowMaskAtCursor();

		}

		private void SetCollisionState(bool allowCollisions)
		{
			_collider.enabled = allowCollisions;
			_rigidbody2D.isKinematic = !allowCollisions;
			if (!allowCollisions)
				_rigidbody2D.linearVelocity = Vector2.zero;
		}

		private void EnterRecoveryState()
		{
			_recoveryEnterTime = Time.time;
		}

		private void ExitRecoveryState()
		{
			_recoveryEnterTime = -1f;
		}

		private bool InRecoveryTooLong()
		{
			return _recoveryEnterTime != -1.0f && Time.time - _recoveryEnterTime >= _maxRecoveryTime;
		}

		#endregion
	}
}
