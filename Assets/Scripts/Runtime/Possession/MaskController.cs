using System;

using DG.Tweening;

using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

using Random = UnityEngine.Random;

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
		[Range(0, 1)]
		private float _healthPercentRequiredToPossessWithoutStun = 0.2f;

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

		private AudioClip maskSqueal;
		
		public GameObject audioInstance;

		private float _inputLockTimestamp;
		private bool _isInputLocked = false;

		private Vector2 _throwDirection;

		private float _maxShakeIntensity = 1f;
		private Vector3 _originalLocalPos = Vector2.zero;

		[SerializeField]
		private Transform _visualsTransform;

		private bool _isInRecovery = false;

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

			CheckForStoppedThrow();
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
				DamageController damageController = other.gameObject.GetComponent<DamageController>();

				// Check their health level to detetrmine if we need to stun the player
				bool shouldStun = damageController.HealthAsPercent > _healthPercentRequiredToPossessWithoutStun;
				Possess(possessable, shouldStun);
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

			ExitRecoveryState();
		}

		private void CheckForStoppedThrow()
		{
			if (_currentPossessedTarget != null)
			{
				return;
			}

			if (_rigidbody2D.linearVelocity.magnitude < 0.1)
			{
				EnterRecoveryState();

				DoRecoveryFeedback();

				if (InRecoveryTooLong())
				{
					GameManager.Instance.RestartLevel();
				}
			}
		}

		private void DoRecoveryFeedback()
		{
			float timeLeft = _maxRecoveryTime - (Time.time - _recoveryEnterTime);
			float progressToDeath = Mathf.Clamp01(1 - (timeLeft / _maxRecoveryTime));
			float currentShake = Mathf.Pow(progressToDeath, 2) * _maxShakeIntensity;

			_visualsTransform.localPosition = _originalLocalPos + (Vector3) Random.insideUnitCircle * currentShake;

			if (progressToDeath >= 1f)
			{
				_visualsTransform.localPosition = _originalLocalPos;
			}
		}

		private void ResetRecoveryFeedback()
		{
			_visualsTransform.localPosition = Vector3.zero;
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

		private void Possess(IPossessable target, bool willStun = true)
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

			ExitRecoveryState();

			RefreshThrows();

			//Play Random Scream Sound
			audioInstance = SoundFXManager.Instance.PlayRandomSoundFXClip(enemyScreams, transform, 1f);
			
			// Lock inputs for stun
			if (willStun)
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
			DamageController possessedTargetDC = _currentPossessedTarget.GetGameObject().GetComponent<DamageController>();
			if(possessedTargetDC != null) {
				possessedTargetDC.Kill();
			}

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
			if (!_isInRecovery)
			{
				_recoveryEnterTime = Time.time;
				_isInRecovery = true;
			}
		}

		private void ExitRecoveryState()
		{
			if (!_isInRecovery)
				return;

			_isInRecovery = false;
			_recoveryEnterTime = -1f;

			ResetRecoveryFeedback();
		}

		private bool InRecoveryTooLong()
		{
			return _isInRecovery && Time.time - _recoveryEnterTime >= _maxRecoveryTime;
		}

		#endregion
	}
}
