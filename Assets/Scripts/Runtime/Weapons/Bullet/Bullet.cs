using UnityEngine;

public class Bullet : MonoBehaviour
{
    private float mDamage;
    private Vector2 mDir;
    private float mVelocity;

    private float mKillTimer;

    private Rigidbody2D mRigidBody;

    public void SetDamage(float damage)
    {
        mDamage = damage;
    }

    public void Fire(Vector2 direction, float velocity)
    {
        mDir = direction;
        mVelocity = velocity;

        Debug.Log("FIRING WITH: " + mVelocity + " AND " + mDir);
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

        DamageController damageController = other.gameObject.GetComponent<DamageController>();

        if(damageController)
        {
            Debug.Log("deal damage");
            damageController.TakeDamage(mDamage);
        }

        Destroy(gameObject);
        
    }
}
