using System.Collections.ObjectModel;
using Winestro_A.Controls;
using Winestro_A.Helpers;

namespace Winestro_A.Services;

public class LogService
{
    public static ObservableCollection<LogMessageControl> LogMessages { get; private set; } = new();
    public static ObservableCollection<LogMessageControl> InfoMessages { get; private set; } = new();
    public static ObservableCollection<LogMessageControl> WarningMessages { get; private set; } = new();
    public static ObservableCollection<LogMessageControl> ErrorMessages { get; private set; } = new();

    public static ObservableCollection<LogMessageControl> MusicMessages { get; private set; } = new();

    public static void Log(string msg)
    {
        LogMessageControl lmc = new()
        {
            Text = msg,
            Type = Enums.LogMessageTypes.Info,
            Time = TimeHelper.NowS()
        };

        LogMessages.Add(lmc);
        InfoMessages.Add(lmc);
    }
    public static void Warning(string msg)
    {
        LogMessageControl lmc = new()
        {
            Text = msg,
            Type = Enums.LogMessageTypes.Warning,
            Time = TimeHelper.NowS()
        };

        LogMessages.Add(lmc);
        WarningMessages.Add(lmc);
    }
    public static void Error(string msg)
    {
        LogMessageControl lmc = new()
        {
            Text = msg,
            Type = Enums.LogMessageTypes.Error,
            Time = TimeHelper.NowS()
        };

        LogMessages.Add(lmc);
        ErrorMessages.Add(lmc);
    }
}
