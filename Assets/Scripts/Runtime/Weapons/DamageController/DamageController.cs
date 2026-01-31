using System;
using Runtime;
using UnityEngine;
using UnityEngine.UI;

public class DamageController : MonoBehaviour
{

    [SerializeField] float mMaxHealth;

    private float mCurHealth;

    public void TakeDamage(float damage)
    {
        mCurHealth -= damage;

        if (mCurHealth <= 0)
        {
            MaskController mask = gameObject.GetComponent<MaskController>();
            Agent agent = gameObject.GetComponent<Agent>();

            if(agent.IsPossessed)
            {
                GameManager.Instance.RestartLevel();
            }

            Destroy(gameObject); // remove this when death state is setup
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
