using UnityEngine;

using UnityEngine.Events;
public class DoorLock : MonoBehaviour
{

    [SerializeField] Rigidbody2D mDoorRigidBody;
    [SerializeField] string mRequiredKey;
    [SerializeField] private UnityEvent mUnlockEvent;


    private void OnCollisionEnter2D(Collision2D other)
    {
        Key collidedKey = other.gameObject.GetComponent<Key>();
        if(collidedKey != null && collidedKey.KeyCode == mRequiredKey)
        {
            mDoorRigidBody.bodyType = RigidbodyType2D.Dynamic;
            mUnlockEvent?.Invoke();
        }
    }
}
