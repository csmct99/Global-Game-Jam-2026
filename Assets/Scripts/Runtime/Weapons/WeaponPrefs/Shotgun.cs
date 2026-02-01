using UnityEngine;

public class Shotgun : WeaponBase
{
	#region Private Fields

	[SerializeField]
	private int mNumPellets;

	[SerializeField]
	private float mSpreadAngle;

	[SerializeField]
	private AudioClip shotgunEmpty;

	[SerializeField]
	private AudioClip gunShotgunFire;

	#endregion

	#region Protected Methods

	protected override void TryFire()
	{
		float timeToBullet = 60.0f / mFireRate;

		if (mCurTime < timeToBullet)
			mCurTime += Time.deltaTime;

		if (mCurTime > timeToBullet)
		{
			if (mAmmoLeft > 0)
			{
				Vector2 firePos = transform.position;

				//Play gunshot sfx
				SoundFXManager.Instance.PlaySoundFXClip(gunShotgunFire, transform, 0.6f);
				if (mFirePosition != null)
				{
					firePos = mFirePosition.transform.position;
				}

				for (int i = 0; i < mNumPellets; i++)
				{
					Quaternion rotationAngleAxis = Quaternion.AngleAxis(-mSpreadAngle / 2 + mSpreadAngle / mNumPellets * i, Vector3.forward);
					Vector3 initDir = rotationAngleAxis * transform.up;

					GameObject bulletObj = Instantiate(mBulletPrefab, (Vector3) firePos, Quaternion.identity);

					bulletObj.transform.up = initDir;

					Bullet firedBullet = bulletObj.GetComponent<Bullet>();
					firedBullet.InitializeBulletData(initDir, mBulletVelocity, mDamage, transform.root.gameObject);
				}

				if (mConsumeAmmo)
				{
					mAmmoLeft -= 1;
					GameManager.Instance.UpdateAmmoState(mAmmoLeft, mMaxAmmo);
				}
			}
			else
			{
				//TODO: Noah put empty gun shot sound here
				SoundFXManager.Instance.PlaySoundFXClip(shotgunEmpty, transform, 1f);
			}

			mCurTime -= timeToBullet;
		}
	}

	#endregion
}
