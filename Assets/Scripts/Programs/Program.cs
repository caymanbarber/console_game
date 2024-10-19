using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Program
{
    protected ComputerMonitor stateController;
    public Program(ComputerMonitor computerMonitor) {
        stateController = computerMonitor;
    }
    public abstract void OnUpdate();

    public abstract void OnStateEnter();

    public abstract void OnStateExit();
}
