
using System;
using UnityEngine;


public class WeaponBase : MonoBehaviour
{

    [SerializeField] public int mDamage;
    [SerializeField] public float mBulletVelocity; 
    [SerializeField] public int mFireRate;
    [SerializeField] public GameObject mBulletPrefab; 
    [SerializeField] public GameObject mFirePosition;

    enum FireState
    {
        IDLE,
        FIRING
    }

    private FireState mCurState;
    
    private float mCurTime;

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
    }

    // Update is called once per frame
    void Update()
    {
        if(mCurState == FireState.FIRING)
        {
            mCurTime += Time.deltaTime;

            float timeToBullet = 60/mFireRate;
            if(mCurTime > timeToBullet)
            {
                Vector2 firePos = transform.position;
                if(mFirePosition != null)
                {
                    firePos = mFirePosition.transform.position;
                }

                GameObject bulletObj = Instantiate(mBulletPrefab, firePos, transform.rotation);
                Bullet firedBullet = bulletObj.GetComponent<Bullet>();
                
                firedBullet.SetDamage(mDamage);
                firedBullet.Fire(transform.up, mBulletVelocity);
                
                mCurTime -= timeToBullet;
            }
        }
        else if(mCurState == FireState.IDLE)
        {
            mCurTime = 0;
        }
    }
}
