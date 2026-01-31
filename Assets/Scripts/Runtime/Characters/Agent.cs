using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Runtime
{
    public abstract class Agent : MonoBehaviour, IPossessable
    {
        protected bool _isPossessed = false;
        public bool IsPossessed => _isPossessed;
           
        [Header("Settings")]
        [Tooltip("Movement speed in units per second.")]
        [SerializeField] protected float _moveSpeed = 5f;
           
        [Tooltip("Rotation speed in degrees per second.")]
        [SerializeField] protected float _rotationSpeed = 720f;
           
        [Header("Dependencies")]
        [Tooltip("The Rigidbody2D component used for movement. Must be set to Kinematic.")]
        [SerializeField] protected Rigidbody2D _rigidbody;

        protected virtual void Awake()
        {
            
        }

        public virtual void BeginPossess(MaskController mask)
        {
            Debug.Log($"Beginning possession of {gameObject.name}");
            _isPossessed = true;

            ConfigureRigidbodyForPossession(_rigidbody, true);

            PlayerControls playerControls = gameObject.AddComponent<PlayerControls>();
            playerControls.Initialize(_moveSpeed, _rotationSpeed, _rigidbody);
        }

        public virtual void StopPossess(MaskController mask)
        {
            Debug.Log($"Stopping possession of {gameObject.name}");

            ConfigureRigidbodyForPossession(_rigidbody, false);
            
            PlayerControls playerControls = GetComponent<PlayerControls>();
            Destroy(playerControls);
            
            _isPossessed = false;
        }

        public virtual void ConfigureRigidbodyForPossession(Rigidbody2D rb, bool isPossessed)
        {
            _rigidbody.isKinematic = !isPossessed;
            _rigidbody.constraints = isPossessed ? RigidbodyConstraints2D.FreezeRotation : RigidbodyConstraints2D.FreezeAll; 
        }

    }
}