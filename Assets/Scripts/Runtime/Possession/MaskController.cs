using System;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.InputSystem;

namespace Runtime
{
    public class MaskController : MonoBehaviour
    {

        private InputAction _throwMaskInput;
        private IPossessable _currentPossessedTarget;

        [SerializeField]
        private Rigidbody2D _rigidbody2D;

        [SerializeField] 
        private Collider2D _collider;

        [SerializeField] private float _throwStrength = 10f;

        [SerializeField] private ParentConstraint _parentConstraint;
        
        [SerializeField] private float _spawnDistanceFromTarget = 2f;

        [SerializeField] private int _maxThrowAttempts = 2;
        private int _currentThrowsRemaining;
        
        public void Update()
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
        }

        private void ThrowMaskAtCursor()
        {
            if (_currentThrowsRemaining <= 0)
            {
                Debug.Log("Out of throws!");
                return;
            }

            _currentThrowsRemaining--;
            
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            Vector2 throwDirection = (mouseWorldPos - transform.position).normalized;
            _rigidbody2D.AddForce(throwDirection * _throwStrength, ForceMode2D.Impulse);
        }

        private void Awake()
        {
            RefreshThrows();
            SetupInputs(); 
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

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (_currentPossessedTarget != null)
            {
                Debug.LogWarning("Got collision event while already possessing a target! ??");
                return;
            }
            
            IPossessable possessable;
            if(TryGetPossessable(other.gameObject, out possessable))
            {
                Possess(possessable);
            }
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
            _parentConstraint.AddSource(new ConstraintSource() { sourceTransform = target.GetGameObject().transform, weight = 1f });
            _parentConstraint.constraintActive = true;
            
            RefreshThrows();
        }

        private void DePossess()
        {
            if (_currentPossessedTarget == null)
            {
                throw new Exception("Tried to de-possess even though we are not currently possessing anything!");
            }
            
            _currentPossessedTarget.StopPossess(this);
            
            // Stop following the possessed target
            _parentConstraint.RemoveSource(0);
            _parentConstraint.constraintActive = false;
            
            //Place the mask in front of the target
            Transform target = _currentPossessedTarget.GetGameObject().transform;
            Vector2 newPos = target.position + target.up * _spawnDistanceFromTarget; // 2 is spawn dist.
            transform.position = newPos;
            
            // Kill the target
            //TODO: Make this more interesting than a "delete"
            Destroy(_currentPossessedTarget.GetGameObject());
            
            // Drop cached reference
            _currentPossessedTarget = null;
            
            // Allow collisions again
            SetCollisionState(true);
            
            // Give throws back
            RefreshThrows();
        }

        private void SetCollisionState(bool allowCollisions)
        {
            _collider.enabled = allowCollisions;
            _rigidbody2D.isKinematic = !allowCollisions;
        }
        
        private void EnterRecoveryState()
        {
            
        }

        private void ExitRecoveryState()
        {
            
        }
        
        
    }
}