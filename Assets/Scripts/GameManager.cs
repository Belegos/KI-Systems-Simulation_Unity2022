using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class GameManager : MonoBehaviour
{
    [FormerlySerializedAs("_healthBar")] [SerializeField] HealthBarScript healthBar;
    public static GameManager _gameManager 
    {
        get;
        private set;
    }

    public UnitHealth PlayerHealth = new UnitHealth(100,100);

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
            healthBar.SetMaxHealth(PlayerHealth.MaxHealth);
            healthBar.SetHealth(PlayerHealth.Health);
    }

    public void UpdateHealthBar(int newCurrentHealth)
    {
        healthBar.SetHealth(newCurrentHealth);
    }


}
