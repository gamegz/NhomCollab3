using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerActionState : MonoBehaviour
{
    #region Non-Serializable
    // References
    protected static PlayerState _state; 
    public PlayerState CurrentState => _state; // Get
    protected void MoveToState(PlayerState newState) // Set
    {
        _state = newState;
    }

    #endregion


    #region Serializable
    [SerializeField] private PlayerState currentState; // To check if player's state is working correctly or not.
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


    private void Update()
    {
        currentState = CurrentState;
    }


    protected virtual void IdleAction() { return;} // Add any universal init here

    protected virtual void MovingAction() { return;} // Add any universal init here

    protected virtual void DashingAction() { return;} // Add any universal init here

    protected virtual void ParryingAction() { return;} // Add any universal init here


}
