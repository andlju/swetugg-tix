using System;
using System.Collections.Generic;
using System.Text;

namespace Swetugg.Tix.Activity.Views
{
    public enum CommandStatus 
    {
        Created,
        Completed,
        Failed
    }

    public enum CommandLogSeverity
    {
        Verbose = 0,
        Debug = 1,
        Information = 2,
        Warning = 3,
        Error = 4
    }
}
