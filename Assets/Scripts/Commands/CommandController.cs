using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CommandController
{
    public Dictionary<String, Command> commandMap;
    public Command defaultCommand;
    public ComputerMonitor stateController; 
    public TypingController typingController;


    //TODO: Add trie for autocomplete
    public CommandController(ComputerMonitor computerMonitor, TypingController typingController) {
        stateController = computerMonitor;
        this.typingController = typingController;
        commandMap = new Dictionary<String, Command>
        {
            //Add commands here
            { "controls", new Attitude(this) },
            { "help", new Help(this) },
            { "screensaver", new ScreenSaverCommand(this) },
            { "video", new VideoFeedCommand(this)},
            { "clear", new Clear(this)}
        };
        defaultCommand = new NotFound(this);
    }

    public bool ParseCommand(String commandName, List<String> args, List<Line> output) {
        try {
            Command command = commandMap[commandName];
            command.RunCommand(args, output);
            
            return true;
        } catch (KeyNotFoundException) {

            output.Add(new Line("Command not recognized ... try 'help'", false, ""));
            return false;
        }
    }

    public List<Command> GetVisibleCommands() {
        return commandMap.Where(command => command.Value.visible == true).Select(kv => kv.Value).ToList();
    }

    public List<Command> GetAllCommands() {
        return commandMap.Select(kv => kv.Value).ToList();
    }
}
