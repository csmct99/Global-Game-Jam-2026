using UnityEngine;

public class Bullet : MonoBehaviour
{
    private float mDamage;
    private Vector2 mDir;
    private float mVelocity;

    private float mKillTimer;

    private Rigidbody2D mRigidBody;

    private GameObject mCreator;

    public void InitializeBulletData(Vector2 direction, float velocity = 100, float damage = 5, GameObject creator = null)
    {
        mDir = direction;
        mVelocity = velocity;
        mDamage = damage;
        mCreator = creator;

    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mKillTimer = 10.0f;
        
        mRigidBody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if(mDir.magnitude > 0 && mVelocity > 0)
        {
            mRigidBody.MovePosition((Vector2)transform.position + mDir * mVelocity * Time.deltaTime);
        }

        mKillTimer -= Time.deltaTime;

        if(mKillTimer <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject != mCreator) // dont hit self while moving
        {
            DamageController damageController = other.gameObject.GetComponent<DamageController>();

            if(damageController)
            {
                damageController.TakeDamage(mDamage);
            }

            Destroy(gameObject);
        }
    }
}
