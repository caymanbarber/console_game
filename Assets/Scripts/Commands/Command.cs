using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;

public abstract class Command
{
    public String commandName;
    protected List<String> args;
    public bool visible;
    public String usage;
    public String help;
    protected CommandController myCommandController;

    public Command(CommandController controller) {
        myCommandController = controller;
    }
    public abstract void RunCommand(List<String> args, List<Line> output); 

    public abstract void ExitCommand();

    protected virtual void PrintOutput(List<Line> output, String lines) {
        String[] line_array = Regex.Split(lines, "\r\n|\r|\n");

        foreach(var line in line_array) {
            output.Insert(0, new Line(line,false, ""));
        }
    }



}
