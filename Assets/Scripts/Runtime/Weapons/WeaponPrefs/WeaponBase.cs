
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
    
    private float mCurTime;
    private int mAmmoLeft;

    public void toggleFire(bool firing = false)
    {
        if(firing)
        {
            mCurState = FireState.FIRING;
        } else {
            mCurState = FireState.IDLE;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mCurState = FireState.IDLE;

        mAmmoLeft = mMaxAmmo;
    }

    // Update is called once per frame
    void Update()
    {
        float timeToBullet = 60.0f / mFireRate;
        
        if(mCurTime < timeToBullet) mCurTime += Time.deltaTime;

        if(mCurState == FireState.FIRING && mAmmoLeft > 0)
        {

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
                mAmmoLeft -= 1;
                
                mCurTime -= timeToBullet;
            }
        }
    }
}
