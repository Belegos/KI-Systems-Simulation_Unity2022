using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehavior : MonoBehaviour
{
    void Start()
    {

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GameManager._gameManager._playerHealth.DamageUnit(10);
            Debug.Log("Player Health: " + GameManager._gameManager._playerHealth.Health);

        }
        if (Input.GetKeyDown(KeyCode.H))
        {
            GameManager._gameManager._playerHealth.HealUnit(10);
            Debug.Log("Player Health: " + GameManager._gameManager._playerHealth.Health);
        }
    }

    private void PlayerTakeDamage(int dmg)
    {
        GameManager._gameManager._playerHealth.DamageUnit(dmg);
    }
    private void PlayerTakeHeal(int heal)
    {
        GameManager._gameManager._playerHealth.HealUnit(heal);
    }
}