using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Text;
using System.Text.RegularExpressions;

public class Help : Command
{
    public Help(CommandController controller):base(controller) {
        commandName = "help";
        usage = "[ ,command]";
        help = "Describe usage for commands or list commands";

        visible = true;
    }
    public override void RunCommand(List<string> args, List<Line> output) {
        this.args = args;
        String content;

        if(args.Count > 0) {
            content = GetHelpArg(args);
        } else {
            content = GetHelpNoArg();
        }

        PrintOutput(output, content);
    }

    private String GetHelpNoArg() {
        StringBuilder sb = new StringBuilder();
        foreach (Command command in myCommandController.GetVisibleCommands()) {
            sb.Append("- " + command.commandName + "  " + command.usage);
            sb.Append(System.Environment.NewLine);
        }

        return sb.ToString();
    }

    private String GetHelpArg(List<string> args) {

        if(args.Count > 1) {
            return "Too many arguments for " + commandName;
        }

        String output;

        try {
            Command argCommand = myCommandController.commandMap[args[0]];
            output = "Command: " + argCommand.commandName + "  " + argCommand.usage +
                        "\n" + argCommand.help + "\n";
        
        } catch (KeyNotFoundException) {
            return "Command argument not recognized"; 
        }

        StringBuilder sb = new StringBuilder();
        foreach (Command command in myCommandController.GetVisibleCommands()) {

            sb.Append("- " + command.commandName + "  " + command.usage);
            sb.Append(System.Environment.NewLine);
        }

        return output;
    }

    public override void ExitCommand() {

    }
}
