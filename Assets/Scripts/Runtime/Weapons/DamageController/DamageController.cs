using System;
using Runtime;
using UnityEngine;
using UnityEngine.UI;

public class DamageController : MonoBehaviour
{

    [SerializeField] float mMaxHealth;
    [SerializeField] private AudioClip hurtSound;
    [SerializeField] private AudioClip deathSound;
    private float mCurHealth;

    private bool mInvuln;

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

    public float HealthAsPercent =>  CurrentHealth / MaxHealth;
    
    public void Kill()
    {
        ToggleInvuln(false);
        TakeDamage(mMaxHealth);
    }

    public void ToggleInvuln(bool invuln)
    {
        mInvuln = invuln;
    }

    public void TakeDamage(float damage)
    {
        if(mInvuln) return;

        mCurHealth = mCurHealth - damage;

        if (mCurHealth <= 0)
        {
            MaskController mask = gameObject.GetComponent<MaskController>();
            Agent agent = gameObject.GetComponent<Agent>();
            
            //Play deathsound
            SoundFXManager.Instance.PlaySoundFXClip(deathSound, transform, 0.3f);

            if(agent.IsPossessed)
            {
                GameManager.Instance.RestartLevel();
            }

            Destroy(gameObject); // remove this when death state is setup
        }
        else if(mCurHealth > mMaxHealth)
        {
            mCurHealth = mMaxHealth;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        mCurHealth = MaxHealth;
    }
}
