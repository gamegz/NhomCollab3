using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FiniteStateMachine.State;

namespace FiniteStateMachine
{
    public class StateMachine
    {
        public StateMachineBaseState _currentState;
              
        //public StateMachine(StateMachineBaseState startState)
        //{
        //    SetStartState(_currentState);
        //}

        public virtual void SetStartState(StateMachineBaseState StartingState)
        {
            _currentState = StartingState;
            _currentState.EnterState();
        }

        public void UpdateState() {
            _currentState.UpdateState();
        }
        public void FixedUpdateState()
        {
            _currentState.FixedUpdateS();
        }

        public void SwitchState(StateMachineBaseState StateToSwitch)
        {
            _currentState.ExitState();
            _currentState = StateToSwitch;
            _currentState.EnterState();
        }
    }
}

