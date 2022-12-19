using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] HealthBarScript _healthBar;
    public static GameManager _gameManager 
    {
        get;
        private set;
    }

    public UnitHealth _playerHealth = new UnitHealth(100,100);

    void Awake()
    {
        if (_gameManager != null && _gameManager != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _gameManager = this;
        }
        DontDestroyOnLoad(this.gameObject);
    }
    private void Start()
    {
            _healthBar.SetMaxHealth(_playerHealth.MaxHealth);
            _healthBar.SetHealth(_playerHealth.Health);
    }

    public void UpdateHealthBar(int newCurrentHealth)
    {
        _healthBar.SetHealth(newCurrentHealth);
    }


}
