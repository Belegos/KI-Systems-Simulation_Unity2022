using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace StateManager
{
    public abstract class State
    {
        //TODO: implement EnterCurrentState and ExitCurrentState
        public abstract State ExecuteCurrentState(StateManager manager); //returns current state und runs it
    }
}
