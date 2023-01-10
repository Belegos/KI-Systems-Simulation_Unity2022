using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitHealth
{
    #region props
    public int Health
    {
        get
        {
            return _currentHealth;
        }
        set 
        {
            _currentHealth = value;
        }
    }    
    public int MaxHealth
    {
        get
        {
            return _currentMaxHealth;
        }
        set 
        {
            _currentMaxHealth = value;
        }
    }
    #endregion

    #region fields
    private int _currentHealth;
    private int _currentMaxHealth;

    #endregion
    // Constructor
    public UnitHealth(int health, int maxHealth)
    {
        _currentHealth = health;
        _currentMaxHealth = maxHealth;
    }
    #region methods
    public void DamageUnit(int dmgAmount)
    {
        if (_currentHealth > 0)
        {
            _currentHealth -= dmgAmount;
        }
    }
    public void HealUnit(int healAmount)
    {
        if (_currentHealth < _currentMaxHealth)
        {
            if (_currentHealth + healAmount > _currentMaxHealth) //prevents to get more health then _currentMaxHealth
            {
                _currentHealth = _currentMaxHealth;
            }
            else
            _currentHealth += healAmount;
        }
    }
    #endregion
}
