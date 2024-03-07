using System.Collections.ObjectModel;
using Winestro_A.Controls;
using Winestro_A.Enums;
using Winestro_A.Helpers;

namespace Winestro_A.Services;

public class LogService
{
    public static ObservableCollection<LogMessageControl> LogMessages { get; private set; } = new();
    public static ObservableCollection<LogMessageControl> InfoMessages { get; private set; } = new();
    public static ObservableCollection<LogMessageControl> WarningMessages { get; private set; } = new();
    public static ObservableCollection<LogMessageControl> ErrorMessages { get; private set; } = new();

    public static ObservableCollection<LogMessageControl> MusicMessages { get; private set; } = new();
    public static ObservableCollection<LogMessageControl> MiscMessages { get; private set; } = new();
    public static ObservableCollection<LogMessageControl> DebugMessages { get; private set; } = new();

    public static void Log(string msg, LogMessageMetaTypes meta = LogMessageMetaTypes.Default)
    {
        _TryLog_(msg, LogMessageTypes.Info, meta);
    }
    public static void Warning(string msg, LogMessageMetaTypes meta = LogMessageMetaTypes.Default)
    {
        _TryLog_(msg, LogMessageTypes.Warning, meta);
    }
    public static void Error(string msg, LogMessageMetaTypes meta = LogMessageMetaTypes.Default)
    {
        _TryLog_(msg, LogMessageTypes.Error, meta);
    }

    private static void _TryLog_(string msg, LogMessageTypes type, LogMessageMetaTypes meta)
    {
        try
        {
            _AddLog_(msg, type, meta);
        } catch (Exception e) { }
    }

    private static void _AddLog_(string msg, LogMessageTypes type, LogMessageMetaTypes meta)
    {
        LogMessageControl lmc = new()
        {
            Text = msg,
            Type = type,
            Time = TimeHelper.NowS(),
            Meta = meta
        };
        LogMessages.Add(lmc);

        LogMessageControl lmce = new()
        {
            Text = msg,
            Type = type,
            Time = TimeHelper.NowS(),
            Meta = meta
        };
        switch (type)
        {
            case LogMessageTypes.Warning:
                WarningMessages.Add(lmce);
                break;
            case LogMessageTypes.Error:
                ErrorMessages.Add(lmce);
                break;
            default:
                if (meta == LogMessageMetaTypes.Default)
                    InfoMessages.Add(lmce);
                break;
        }

        LogMessageControl lmcmeta = new()
        {
            Text = msg,
            Type = type,
            Time = TimeHelper.NowS(),
            Meta = meta
        };
        switch (meta)
        {
            case LogMessageMetaTypes.Music:
                MusicMessages.Add(lmcmeta);
                break;
            case LogMessageMetaTypes.Misc:
                MiscMessages.Add(lmcmeta);
                break;
            case LogMessageMetaTypes.Debug:
                DebugMessages.Add(lmcmeta);
                break;
            default:
                break;
        }
    }
}
