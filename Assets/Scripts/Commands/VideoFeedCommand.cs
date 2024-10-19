using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class VideoFeedCommand : Command
{
    public VideoFeedCommand (CommandController controller):base(controller) {
        commandName = "video";
        visible = true;
        usage = "[ ]";
        help = "See camera video feed";
    }

    public override void RunCommand(List<String> args, List<Line> output) {
        myCommandController.stateController.EnterState("VideoFeed");
    }


    public override void ExitCommand() {

    }
}
