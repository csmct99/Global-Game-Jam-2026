using System;

using UnityEngine;
using UnityEngine.InputSystem;

namespace Runtime
{
	public abstract class Agent : MonoBehaviour, IPossessable
	{
		#region Protected Fields

		protected bool _isPossessed = false;
        [SerializeField] private WeaponBase _weapon;

		[Header("Settings")]
		[Tooltip("Movement speed in units per second.")]
		[SerializeField]
		protected float _moveSpeed = 5f;

		[Tooltip("Rotation speed in degrees per second.")]
		[SerializeField]
		protected float _rotationSpeed = 720f;

		[Header("Dependencies")]
		[Tooltip("The Rigidbody2D component used for movement. Must be set to Kinematic.")]
		[SerializeField]
		protected Rigidbody2D _rigidbody;


		#endregion

		#region Properties
		public bool IsPossessed => _isPossessed;

		#endregion

		#region MonoBehaviour Methods

		protected virtual void Awake()
		{
		}

		#endregion

		#region Public Methods

		public virtual void BeginPossess(MaskController mask)
		{
			Debug.Log($"Beginning possession of {gameObject.name}");
			_isPossessed = true;

			ConfigureRigidbodyForPossession(_rigidbody, true);

            PlayerControls playerControls = gameObject.AddComponent<PlayerControls>();
            playerControls.Initialize(_moveSpeed, _rotationSpeed, _rigidbody, this);

            GameManager.Instance.possessedAgent = this;
		}

		public virtual void ConfigureRigidbodyForPossession(Rigidbody2D rb, bool isPossessed)
		{
			_rigidbody.isKinematic = !isPossessed;
			_rigidbody.constraints = isPossessed ? RigidbodyConstraints2D.FreezeRotation : RigidbodyConstraints2D.FreezeAll;
		}

		public virtual void StopPossess(MaskController mask)
		{
			Debug.Log($"Stopping possession of {gameObject.name}");

			ConfigureRigidbodyForPossession(_rigidbody, false);

			PlayerControls playerControls = GetComponent<PlayerControls>();
			Destroy(playerControls);

			_isPossessed = false;

            ToggleWeaponFire(false); // prevent auto fire after leaving
            GameManager.Instance.possessedAgent = null;
		}


        public void ToggleWeaponFire(bool fire)
        {
            if(_weapon != null)
            {
                _weapon.toggleFire(fire);
            }
        }

		#endregion

    }
}
