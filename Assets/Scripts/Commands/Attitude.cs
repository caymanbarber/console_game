using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Text;
using System.Text.RegularExpressions;

public class Attitude : Command
{
    public Attitude(CommandController controller):base(controller) {
        commandName = "controls";
        visible = true;
        usage = "[ ]";
        help = "Control vessel attitude";
    }
    public override void RunCommand(List<string> args, List<Line> output) {
        myCommandController.stateController.EnterState("Controls");
    }

    public override void ExitCommand() {

    }


}
