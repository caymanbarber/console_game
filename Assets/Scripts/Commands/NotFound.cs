using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Text;
using System.Text.RegularExpressions;


public class NotFound : Command
{
    public NotFound(CommandController controller):base(controller) {
        commandName = "NotFound";
        visible = true;
    }
    public override void RunCommand(List<string> args, List<Line> output) {
        this.args = args;
        String content = "Command not found ... try 'help'";
        PrintOutput(output, content);
    }

    public override void ExitCommand() {

    }
}
