using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IStateMachineBaseState
{
    public void EnterState();
    public void UpdateState();
    public void FixedUpdateS();
    public void ExitState();
}
