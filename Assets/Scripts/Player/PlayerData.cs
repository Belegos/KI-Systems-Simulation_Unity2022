using System;
using System.ComponentModel;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
//using UnityEngine.UI;

namespace StateManager
{
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
                PropertyChanged(this, new PropertyChangedEventArgs("Health"));
            }
        }

        public PropertyChangedEventHandler OnTakeDamage
        {
            get
            {
                return PropertyChanged;
            }
            set
            {
                if (value != null)
                {
                    TakeDamage(1);
                    PropertyChanged(this, new PropertyChangedEventArgs("Health"));
                }
            }
        }

        private void OnEnable()
        {
            PropertyChanged += OnTakeDamage;

        }
        private void OnDisable()
        {
            PropertyChanged -= OnTakeDamage;
        }
        private void Start()
        {
            
            Health = _maxHealth;
        }

        public void TakeDamage(int damage)
        {
            Health -= damage;
            PropertyChanged(this, new PropertyChangedEventArgs("Health"));
        }

    }
}

