using System;
using System.Collections.Generic;
using System.Text;

namespace MyDairy.Commands;

public class ActionCommand(Action execute, Action undo)
{
    public void Execute() => execute();
    public void Undo() => undo();
}
