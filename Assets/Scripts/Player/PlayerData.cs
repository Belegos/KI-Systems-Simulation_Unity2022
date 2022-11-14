using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData : MonoBehaviour
{
    [SerializeField][Range(0,100)] private int _currentHealth;
    [SerializeField][Range(0, 100)] private int _maxHealth;


    public int Health { get => _currentHealth; set => _currentHealth = value; }

    public void TakeDamage(int damage)
    {
        Health -= damage;
    }
    private void Start()
    {
        Health = _maxHealth;
    }
}
