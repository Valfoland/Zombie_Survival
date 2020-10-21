using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    [SerializeField] protected float StartingHealth;
    protected float currentHealth;

    public bool Dead;

    protected virtual void Start()
    {
        currentHealth = StartingHealth;
    }

    public virtual void TakeDamage(float amount)
    {
        currentHealth -= amount;
    }
}
