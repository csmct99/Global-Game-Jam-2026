using UnityEngine;

namespace Runtime
{
	public class EnemyAgent : Agent
    {
        [SerializeField] private float mPerceptionDistance;
        private Agent target;

        private void UpdateTarget()
        {
            Agent potentialTarget = GameManager.Instance.possessedAgent;

            if(potentialTarget!= null)
            {
                Vector2 targetPos = potentialTarget.gameObject.transform.position;
                float dist = Vector2.Distance(targetPos, gameObject.transform.position);

                if(dist < mPerceptionDistance)
                {
                    
                    target = potentialTarget;
                    return;
                }
            }
            target = null;
        }

        private bool UpdateRotation()
        {
            if (_rigidbody == null || target == null) return false;

		    Vector2 direction = target.gameObject.transform.position - transform.position;

		    float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

		    targetAngle -= 90f;

		    // Smoothly rotate towards the target angle
		    Quaternion targetRotation = Quaternion.Euler(0, 0, targetAngle);
		    transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, _rotationSpeed * Time.fixedDeltaTime);
            return true;
        }

        private void UpdateFiringState()
        {
            // TODO: only fire if they're in front of us and no walls (raycast)
            ToggleWeaponFire(target != null, false);
        }

        void FixedUpdate()
        {
            if(IsPossessed) return;
            
            UpdateTarget();

            if(UpdateRotation())
            {
                UpdateFiringState();
            }
            
            UpdateFiringState();
        }


    }
}
