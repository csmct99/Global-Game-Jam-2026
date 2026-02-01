
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
    [SerializeField] private AudioClip gunRevolverEmpty;
    enum FireState
    {
        IDLE,
        FIRING
    }

    private FireState mCurState;
    
    protected float mCurTime;
    protected float mLastFireTime = float.MinValue ;
    
    protected int mAmmoLeft;
    protected bool mConsumeAmmo;


    public int GetMaxAmmo()
    {
        return mMaxAmmo;
    }

    public int GetCurAmmo()
    {
        return mAmmoLeft;
    }

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
        if(Time.time - mLastFireTime > 60/mFireRate)
        {
            Vector2 firePos = transform.position;
            if(mFirePosition != null)
            {
                firePos = mFirePosition.transform.position;
            }

            if(mAmmoLeft > 0)
            {
                GameObject bulletObj = Instantiate(mBulletPrefab, firePos, transform.rotation);
                //Play gunshot sfx
                SoundFXManager.Instance.PlaySoundFXClip(gunRevolverFire, transform, 0.1f);
                
                Bullet firedBullet = bulletObj.GetComponent<Bullet>();
                
                firedBullet.InitializeBulletData(transform.up, mBulletVelocity, mDamage, transform.root.gameObject);
                if(mConsumeAmmo)
                {
                    mAmmoLeft -= 1;
					GameManager.Instance.UpdateAmmoState(mAmmoLeft, mMaxAmmo);
                }
            } else
            {
                //TODO: Noah put empty gun shot sound here
                SoundFXManager.Instance.PlaySoundFXClip(gunRevolverEmpty, transform, 1f);
            }

            mLastFireTime = Time.time;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(mCurState == FireState.FIRING)
        {
            
            TryFire();
        }
    }
}
