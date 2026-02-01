using System;

using DG.Tweening;

using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Serialization;

using Random = UnityEngine.Random;

namespace Runtime
{
	public class MaskController : MonoBehaviour
	{
		#region Public Fields

		public GameObject audioInstance;

		#endregion

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

		[SerializeField]
		private float _healHostAmount = 25.0f;

		[SerializeField]
		private float _invulnTimeAfterStunPosses = 0.5f;

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

		[Header("Visuals")]
		[SerializeField]
		private Volume _damageIndicatorVolume;

		[SerializeField]
		private float _damageEffectFadeFactor = 0.5f;

		[SerializeField]
		private float _damageEffectFadeInSpeedInSeconds = 0.25f;

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

		[SerializeField]
		private AudioClip maskSqueal;

		[SerializeField]
		private AudioClip maskLeap;

		private float _inputLockTimestamp;
		private bool _isInputLocked = false;

		private Vector2 _throwDirection;

		private float _maxShakeIntensity = 1f;
		private Vector3 _originalLocalPos = Vector2.zero;

		[SerializeField]
		private Transform _visualsTransform;

		private bool _isInRecovery = false;

		private DamageController _damageController;

		private int _possessionsCount = 0;

		#endregion

		#region MonoBehaviour Methods

		private void Awake()
		{
			_possessionsCount = 0;
			RefreshThrows();
			SetupInputs();
			UnlockInputs();
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

			if (_currentPossessedTarget != null && _damageController != null && Math.Abs(_damageIndicatorVolume.weight - (1 - _damageController.HealthAsPercent)) > 0.01f)
			{
				_damageIndicatorVolume.weight = Mathf.MoveTowards(_damageIndicatorVolume.weight, 1 - _damageController.HealthAsPercent, Time.deltaTime / _damageEffectFadeInSpeedInSeconds);
			}
			else
			{
				if (_damageIndicatorVolume != null && _damageIndicatorVolume.weight != 0)
				{
					_damageIndicatorVolume.weight = Mathf.MoveTowards(_damageIndicatorVolume.weight, 0, Time.deltaTime / _damageEffectFadeFactor);
				}
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
				_damageController = other.gameObject.GetComponent<DamageController>();

				// Check their health level to detetrmine if we need to stun the player
				bool shouldStun = _damageController.HealthAsPercent > _healthPercentRequiredToPossessWithoutStun;
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
			if (_damageController != null)
				_damageController.MakeInvulnerableForSeconds(_invulnTimeAfterStunPosses);
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
			else
			{
				SoundFXManager.Instance.PlaySoundFXClip(maskLeap, transform, 1f);
			}

			Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
			_throwDirection = (mouseWorldPos - (Vector2) transform.position).normalized;

			float initialSpeed = (2 * _throwDistance) / _throwDuration * _currentThrowsRemaining / _maxThrowAttempts;

			_currentThrowsRemaining--;

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

			_damageController = _currentPossessedTarget.GetGameObject().GetComponent<DamageController>();
			if (_damageController != null)
				_damageController.TakeDamage(-_healHostAmount);

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
			audioInstance = SoundFXManager.Instance.PlayRandomSoundFXClip(enemyScreams, transform, 6f);

			// Lock inputs for stun
			if (willStun && _possessionsCount > 0)
				LockInputs();

			_possessionsCount++;

			if (_currentPossessedTarget != null)
			{
				Agent possessedAgent = _currentPossessedTarget.GetGameObject().GetComponent<Agent>();
				if (possessedAgent != null)
				{
					int curAmmo = possessedAgent.GetCurAmmo();
					int maxAmmo = possessedAgent.GetMaxAmmo();

					GameManager.Instance.UpdateAmmoState(curAmmo, maxAmmo);
				}
			}
		}

		private void DePossess()
		{
			if (_currentPossessedTarget == null)
			{
				throw new Exception("Tried to de-possess even though we are not currently possessing anything!");
			}

			if (audioInstance != null)
				Destroy(audioInstance);

			_currentPossessedTarget.StopPossess(this);

			// Stop following the possessed target
			_parentConstraint.RemoveSource(0);
			_parentConstraint.constraintActive = false;

			// Kill the target
			// TODO: Make this more interesting than a "delete"
			DamageController possessedTargetDC = _currentPossessedTarget.GetGameObject().GetComponent<DamageController>();
			if (possessedTargetDC != null)
			{
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

			GameManager.Instance.DisableAmmoUI();
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
				// TODO: noah put scream sound here
				_recoveryEnterTime = Time.time;
				_isInRecovery = true;
			}
		}

		private void ExitRecoveryState()
		{
			if (!_isInRecovery)
				return;

			// TODO: noah remove scream here if needed

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
