using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerActionState : MonoBehaviour
{
    #region Non-Serializable
    // References
    protected PlayerState _state;

    #endregion


    #region Serializable

    #endregion



    public enum PlayerState
    {
        Idle,
        Moving,
        Dashing,
        Parrying,
    }

    private void Awake()
    {
        #region UNIVERSAL INIT

        MoveToState(PlayerState.Idle);

        #endregion
    }

    protected virtual void IdleAction() { return;} // Add any universal init here

    protected virtual void MovingAction() { return;} // Add any universal init here

    protected virtual void DashingAction() { return;} // Add any universal init here

    protected virtual void ParryingAction() { return;} // Add any universal init here

    protected void MoveToState(PlayerState newState) // [DUC ANH]: THIS SHIT IS GOING PROTECTED LETS GOO 
    {
        _state = newState;

        switch (newState)
        {
            case PlayerState.Idle:
                break;
            case PlayerState.Moving:
                break;
            case PlayerState.Dashing:
                break;
            case PlayerState.Parrying:
                break;
            default:
                break;
        }
    }


}
