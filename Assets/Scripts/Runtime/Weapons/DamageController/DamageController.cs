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
        TakeDamage(mMaxHealth);
    }

    public void TakeDamage(float damage)
    {
        mCurHealth = CurrentHealth - damage;

        if (CurrentHealth <= 0)
        {
            MaskController mask = gameObject.GetComponent<MaskController>();
            Agent agent = gameObject.GetComponent<Agent>();
            
            //Play deathsound
            SoundFXManager.instance.PlaySoundFXClip(deathSound, transform, 1f);

            if(agent.IsPossessed)
            {
                GameManager.Instance.RestartLevel();
            }

            Destroy(gameObject); // remove this when death state is setup
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        mCurHealth = MaxHealth;
    }
}
