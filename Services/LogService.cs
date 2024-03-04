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
    public static Stack<LogMessageControl> LogMessages { get; private set; } = new();
    public static Stack<LogMessageControl> InfoMessages { get; private set; } = new();
    public static Stack<LogMessageControl> WarningMessages { get; private set; } = new();
    public static Stack<LogMessageControl> ErrorMessages { get; private set; } = new();

    public static void Log(string msg)
    {
        LogMessageControl lmc = new()
        {
            Text = msg,
            Type = Enums.LogMessageTypes.Info,
            Time = TimeHelper.NowS()
        };

        LogMessages.Push(lmc);
        InfoMessages.Push(lmc);
    }
    public static void Warning(string msg)
    {
        LogMessageControl lmc = new()
        {
            Text = msg,
            Type = Enums.LogMessageTypes.Warning,
            Time = TimeHelper.NowS()
        };

        LogMessages.Push(lmc);
        WarningMessages.Push(lmc);
    }
    public static void Error(string msg)
    {
        LogMessageControl lmc = new()
        {
            Text = msg,
            Type = Enums.LogMessageTypes.Error,
            Time = TimeHelper.NowS()
        };

        LogMessages.Push(lmc);
        ErrorMessages.Push(lmc);
    }
}
