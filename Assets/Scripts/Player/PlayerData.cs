using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerData : MonoBehaviour, INotifyPropertyChanged
{
    [SerializeField][Range(0, 100)] private int _currentHealth;
    [SerializeField][Range(0, 100)] private int _maxHealth;
    [SerializeField] private Slider slider;
    [SerializeField] private TextMeshPro textComp;
    [SerializeField] private GameObject SliderGameObject;

    public event PropertyChangedEventHandler PropertyChanged;
    public int Health
    {
        get { return _currentHealth; }

        set
        {
            _currentHealth = value;
        }
    }


    public void TakeDamage(int damage)
    {
        Health -= damage;
        PropertyChanged(this, new PropertyChangedEventArgs("Health"));
    }
    private void Awake()
    {
        slider = GetComponent<Slider>();
        textComp = GetComponentInParent<TextMeshPro>();
    }
    private void Start()
    {
        PropertyChanged(this, new PropertyChangedEventArgs("Health"));
        Health = _maxHealth;
    }
}
