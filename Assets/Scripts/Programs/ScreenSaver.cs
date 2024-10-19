using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenSaver : Program
{
    GameObject image;
    public ScreenSaver(ComputerMonitor computerMonitor):base(computerMonitor) {
        image = stateController.transform.Find("Image").gameObject;
    }
    public override void OnUpdate() {
        if(Input.anyKeyDown) {
            stateController.EnterState("CLI");
        }
    }

    public override void OnStateEnter() {
        image.SetActive(true);
    }

    public override void OnStateExit() {
        image.SetActive(false);
    }
}
