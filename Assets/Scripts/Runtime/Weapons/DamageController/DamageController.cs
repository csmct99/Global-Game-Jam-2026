using System;
using UnityEngine;

public class DamageController : MonoBehaviour
{

    [SerializeField] float mMaxHealth;

    private float mCurHealth;

    public void TakeDamage(float damage)
    {
        mCurHealth -= damage;

        if (mCurHealth <= 0)
        {
            // TODO: death anim/vfx/end game screen
            Destroy(gameObject);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mCurHealth = mMaxHealth;

    }

    // Update is called once per frame
    void Update()
    {

        
    }
}
