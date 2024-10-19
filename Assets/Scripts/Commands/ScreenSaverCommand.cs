using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class ScreenSaverCommand : Command
{
    public ScreenSaverCommand (CommandController controller):base(controller) {
        commandName = "screensaver";
        visible = false;
        usage = "[ ]";
        help = "Screen saver for testing";
    }

    public override void RunCommand(List<String> args, List<Line> output) {
        myCommandController.stateController.EnterState("ScreenSaver");
    }


    public override void ExitCommand() {

    }

}
