using System;

using Runtime;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DamageController : MonoBehaviour
{
	#region Public Fields

	public Action<float> OnTookDamage;
	public Action OnHealthChanged;

	public UnityEvent OnDeath;

	#endregion

	#region Private Fields

	[SerializeField]
	float mMaxHealth;

	[SerializeField]
	private AudioClip hurtSound;

	[SerializeField]
	private AudioClip deathSound;

	private float mCurHealth;

	private bool mInvuln;

	private bool dying = false;

	#endregion

	#region Properties

	public float CurrentHealth
	{
		get
		{
			return mCurHealth;
		}
	}

	public float MaxHealth
	{
		get
		{
			return mMaxHealth;
		}
	}

	public float HealthAsPercent => CurrentHealth / MaxHealth;

	#endregion

	#region MonoBehaviour Methods

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Awake()
	{
		mCurHealth = MaxHealth;
	}

	#endregion

	#region Public Methods

	public void DisableInvlun()
	{
		ToggleInvuln(false);
	}

	public void EnableInvlun()
	{
		ToggleInvuln(true);
	}

	public void Kill()
	{
		ToggleInvuln(false);
		TakeDamage(mMaxHealth);
	}

	public void MakeInvulnerableForSeconds(float seconds)
	{
		ToggleInvuln(true);
		Invoke(nameof(DisableInvlun), seconds);
	}

	public void TakeDamage(float damage)
	{
		if (mInvuln || dying)
			return;

		mCurHealth -= damage;

		if (mCurHealth <= 0)
		{
			MaskController mask = gameObject.GetComponent<MaskController>();
			Agent agent = gameObject.GetComponent<Agent>();

			//Play deathsound
			SoundFXManager.Instance.PlaySoundFXClip(deathSound, transform, 0.3f);
			OnDeath?.Invoke();

			if (agent.IsPossessed) // Possessed, restart level
			{
				GameManager.Instance.RestartLevel();
			}
			else // Not possessed, track the global enemy count
			{
				GameManager.Instance.NotifyEnemyKilled();
				Destroy(gameObject); // remove this when death state is setup
				dying = true;
			}
		}
		else if (mCurHealth > mMaxHealth)
		{
			mCurHealth = mMaxHealth;
		}

		if (damage > 0)
			OnTookDamage?.Invoke(damage);
		OnHealthChanged?.Invoke();
	}

	public void ToggleInvuln(bool invuln)
	{
		mInvuln = invuln;
	}

	#endregion
}
