using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Winestro_A.Controls;
using Winestro_A.Helpers;
using Winestro_A.Structures;

namespace Winestro_A.Services;

public class LogService
{
    public static Stack<LogMessage> LogMessages { get; private set; } = new();
    public static Stack<LogMessage> InfoMessages { get; private set; } = new();
    public static Stack<LogMessage> WarningMessages { get; private set; } = new();
    public static Stack<LogMessage> ErrorMessages { get; private set; } = new();

    public static void Log(string msg)
    {
        LogMessage lmsg = new()
        {
            Text = msg,
            Type = Enums.LogMessageTypes.Info,
            Time = TimeHelper.Now
        };

        LogMessages.Push(lmsg);
        InfoMessages.Push(lmsg);
    }
    public static void Warning(string msg)
    {
        LogMessage lmsg = new()
        {
            Text = msg,
            Type = Enums.LogMessageTypes.Warning,
            Time = TimeHelper.Now
        };

        LogMessages.Push(lmsg);
        WarningMessages.Push(lmsg);
    }
    public static void Error(string msg)
    {
        LogMessage lmsg = new()
        {
            Text = msg,
            Type = Enums.LogMessageTypes.Error,
            Time = TimeHelper.Now
        };

        LogMessages.Push(lmsg);
        ErrorMessages.Push(lmsg);
    }
}
