using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FiniteStateMachine;
using FiniteStateMachine.State;

namespace Enemy.statemachine
{
    public class EnemyStateMachine : StateMachine
    {
        public EnemyStateMachine(StateMachineBaseState startState) : base(startState)
        {
            SetStartState(startState);
        }
    }

}
