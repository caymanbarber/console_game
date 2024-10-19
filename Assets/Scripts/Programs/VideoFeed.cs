using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using TMPro;

public class VideoFeed : Program
{
    GameObject cameraFootage;
    public TMP_Text controlBoolText;
    Controller controller;
    public VideoFeed(ComputerMonitor computerMonitor):base(computerMonitor) {
        cameraFootage = stateController.transform.Find("CameraFootage").gameObject;
        controlBoolText = computerMonitor.gameObject.transform.Find("CameraFootage").Find("ControlBool").gameObject.GetComponent<TMP_Text>();
        
    }

    public override void OnUpdate() {
        if(Input.GetKeyDown(KeyCode.Escape)) {
            stateController.EnterState("CLI");
        }

        WriteControls();

        if(stateController.videoControl) {
            
            controller.ControlVessel();
        }
        // on space get new image
        // scan lines
        // take time
    }

    public override void OnStateEnter() {
        controller = (Controller)stateController.programs["Controls"];
        cameraFootage.SetActive(true);
        
    }

    public override void OnStateExit() {
        cameraFootage.SetActive(false);
    }

    void WriteControls() {
        if (stateController.videoControl) {
            controlBoolText.text = "[x] controls";
        } else {
            controlBoolText.text = "[ ] controls";
        }
    }
}
