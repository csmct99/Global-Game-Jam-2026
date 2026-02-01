using UnityEngine;

public class Shotgun : WeaponBase
{

    [SerializeField] private int mNumPellets;
    [SerializeField] private float mSpreadAngle;

    protected override void TryFire()
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

            for(int i = 0; i < mNumPellets; i++)
            {
                Quaternion rotationAngleAxis = Quaternion.AngleAxis(-mSpreadAngle/2 + mSpreadAngle/mNumPellets * i, Vector3.forward);
                Vector3 initDir = rotationAngleAxis * transform.up;

                Debug.Log("Rot angle: " + rotationAngleAxis);
                
                GameObject bulletObj = Instantiate(mBulletPrefab, (Vector3)firePos, Quaternion.identity);

                bulletObj.transform.up = initDir;
                
                Bullet firedBullet = bulletObj.GetComponent<Bullet>();
                firedBullet.InitializeBulletData(initDir, mBulletVelocity, mDamage, transform.root.gameObject);

                Debug.Log(initDir + " " + mBulletVelocity);
            }


			if (mConsumeAmmo)
			{
				mAmmoLeft -= 1;
			}
			
            mCurTime -= timeToBullet;
        }
    }


}
