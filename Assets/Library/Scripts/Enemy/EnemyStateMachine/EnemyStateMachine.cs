using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FiniteStateMachine;
using FiniteStateMachine.State;

namespace Enemy.statemachine
{
    //Handle transitionings, set up and updating states
    public class EnemyStateMachine : StateMachine
    {

        public override void SetStartState(StateMachineBaseState StartingState)
        {
            base.SetStartState(StartingState);
        }


    }

}
