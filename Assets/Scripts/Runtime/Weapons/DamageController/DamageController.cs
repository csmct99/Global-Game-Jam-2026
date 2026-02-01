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

    public Action<float> OnTookDamage;
    public Action OnHealthChanged;
    
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

    private bool dying = false;
    public void TakeDamage(float damage)
    {
        if(mInvuln || dying) return;

        mCurHealth -= damage;

        if (mCurHealth <= 0)
        {
            MaskController mask = gameObject.GetComponent<MaskController>();
            Agent agent = gameObject.GetComponent<Agent>();
            
            //Play deathsound
            SoundFXManager.Instance.PlaySoundFXClip(deathSound, transform, 0.3f);

            if(agent.IsPossessed) // Possessed, restart level
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
        else if(mCurHealth > mMaxHealth)
        {
            mCurHealth = mMaxHealth;
        }
        
        if(damage > 0) OnTookDamage?.Invoke(damage);
        OnHealthChanged?.Invoke();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        mCurHealth = MaxHealth;
    }
}
