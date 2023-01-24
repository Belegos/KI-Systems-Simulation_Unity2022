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
            GameManager._gameManager.PlayerHealth.DamageUnit(10);
            Debug.Log("Player Health: " + GameManager._gameManager.PlayerHealth.Health);

        }
        if (Input.GetKeyDown(KeyCode.H))
        {
            GameManager._gameManager.PlayerHealth.HealUnit(10);
            Debug.Log("Player Health: " + GameManager._gameManager.PlayerHealth.Health);
        }
    }

    private void PlayerTakeDamage(int dmg)
    {
        GameManager._gameManager.PlayerHealth.DamageUnit(dmg);
    }
    private void PlayerTakeHeal(int heal)
    {
        GameManager._gameManager.PlayerHealth.HealUnit(heal);
    }
}