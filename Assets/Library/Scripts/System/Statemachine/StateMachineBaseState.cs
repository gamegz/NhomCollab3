using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FiniteStateMachine.State
{
    public abstract class StateMachineBaseState : MonoBehaviour
    {
        
        public abstract void EnterState();
        public abstract void UpdateState();
        public abstract void FixedUpdateS();
        public abstract void ExitState();

    }

}

