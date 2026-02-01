
using System;
using UnityEngine;


public class WeaponBase : MonoBehaviour
{

    [SerializeField] public int mDamage;
    [SerializeField] public float mBulletVelocity; 
    [SerializeField] public float mFireRate;
    [SerializeField] public GameObject mBulletPrefab; 
    [SerializeField] public GameObject mFirePosition;
    [SerializeField] public int mMaxAmmo;

    [SerializeField] private AudioClip gunRevolverFire;

    enum FireState
    {
        IDLE,
        FIRING
    }

    private FireState mCurState;
    
    protected float mCurTime;
    protected int mAmmoLeft;
    private bool mConsumeAmmo;

    public void toggleFire(bool firing = false, bool consumeAmmo = false)
    {
        if(firing)
        {
            mCurState = FireState.FIRING;
        } else {
            mCurState = FireState.IDLE;
        }

        mConsumeAmmo = consumeAmmo;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mCurState = FireState.IDLE;

        mAmmoLeft = mMaxAmmo;
    }

    protected virtual void TryFire()
    {
        float timeToBullet = 60.0f / mFireRate;
        
        if(mCurTime < timeToBullet) mCurTime += Time.deltaTime;

        if(mCurTime > timeToBullet)
        {
            Vector2 firePos = transform.position;
            if(mFirePosition != null)
            {
                firePos = mFirePosition.transform.position;
            }

            GameObject bulletObj = Instantiate(mBulletPrefab, firePos, transform.rotation);
            Bullet firedBullet = bulletObj.GetComponent<Bullet>();
            
            firedBullet.InitializeBulletData(transform.up, mBulletVelocity, mDamage, transform.root.gameObject);
            if(mConsumeAmmo)
            {
                mAmmoLeft -= 1;
            }
            
            mCurTime -= timeToBullet;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(mCurState == FireState.FIRING && mAmmoLeft > 0)
        {
            TryFire();
        }
    }
}
