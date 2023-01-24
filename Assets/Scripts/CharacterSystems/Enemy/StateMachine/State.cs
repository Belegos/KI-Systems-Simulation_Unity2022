using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace StateManager
{
    public abstract class State
    {
        public abstract State ExecuteCurrentState(StateManager manager); //returns current state und runs it
    }
}
