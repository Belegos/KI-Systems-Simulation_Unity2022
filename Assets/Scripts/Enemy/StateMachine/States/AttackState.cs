using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : State
{
    public override State ExecuteCurrentState()
    {
        Debug.Log("Player have been attacked");
        return this;
    }

}
