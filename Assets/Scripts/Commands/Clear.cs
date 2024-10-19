using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clear : Command
{
    public Clear(CommandController controller):base(controller) {
        commandName = "clear";
        visible = true;
        usage = "[ ]";
        help = "Clear CLI lines";
    }

    public override void RunCommand(List<string> args, List<Line> output) {
       myCommandController.typingController.ClearLines();
    }

    public override void ExitCommand() {

    }
}
