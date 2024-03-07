using System.Collections.ObjectModel;
using Microsoft.VisualBasic.Logging;
using Winestro_A.Controls;
using Winestro_A.Enums;
using Winestro_A.Helpers;
using Winestro_A.Structures;

namespace Winestro_A.Services;

public class LogService
{
    private static List<LogMessageManifest> logs = new();

    public static ObservableCollection<LogMessageControl> LogMessages { get; private set; } = new();
    public static ObservableCollection<LogMessageControl> InfoMessages { get; private set; } = new();
    public static ObservableCollection<LogMessageControl> WarningMessages { get; private set; } = new();
    public static ObservableCollection<LogMessageControl> ErrorMessages { get; private set; } = new();

    public static ObservableCollection<LogMessageControl> MusicMessages { get; private set; } = new();
    public static ObservableCollection<LogMessageControl> MiscMessages { get; private set; } = new();
    public static ObservableCollection<LogMessageControl> DebugMessages { get; private set; } = new();

    public static bool UpdateRequired = false;

    public static void Log(string msg, LogMessageMetaTypes meta = LogMessageMetaTypes.Default)
    {
        TryLog(msg, LogMessageTypes.Info, meta);
    }
    public static void Warning(string msg, LogMessageMetaTypes meta = LogMessageMetaTypes.Default)
    {
        TryLog(msg, LogMessageTypes.Warning, meta);
    }
    public static void Error(string msg, LogMessageMetaTypes meta = LogMessageMetaTypes.Default)
    {
        TryLog(msg, LogMessageTypes.Error, meta);
    }

    private static void TryLog(string msg, LogMessageTypes type, LogMessageMetaTypes meta)
    {
        try
        {
            logs.Add(new LogMessageManifest() { Text=msg, Type=type, Meta=meta, Time=TimeHelper.NowS(), Id=(ulong)logs.Count });
            _UpdateControlsCollections_();
        } catch (Exception e) { UpdateRequired = true; return; }
        UpdateRequired = false;
    }

    private static void AddLog(LogMessageManifest log)
    {
        LogMessageControl lmc = new()
        {
            Text = log.Text,
            Type = log.Type,
            Time = log.Time,
            Meta = log.Meta,
            Id = log.Id,
        };
        LogMessages.Add(lmc);

        LogMessageControl lmce = new()
        {
            Text = log.Text,
            Type = log.Type,
            Time = log.Time,
            Meta = log.Meta,
            Id = log.Id,
        };
        switch (log.Type)
        {
            case LogMessageTypes.Warning:
                WarningMessages.Add(lmce);
                break;
            case LogMessageTypes.Error:
                ErrorMessages.Add(lmce);
                break;
            default:
                if (log.Meta == LogMessageMetaTypes.Default)
                    InfoMessages.Add(lmce);
                break;
        }

        LogMessageControl lmcmeta = new()
        {
            Text = log.Text,
            Type = log.Type,
            Time = log.Time,
            Meta = log.Meta,
            Id = log.Id,
        };
        switch (log.Meta)
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

    private static void _UpdateControlsCollections_()
    {
        foreach (var log in logs)
        {
            var found = false;
            foreach (var control in LogMessages)
            {
                if (control.Id == log.Id)
                {
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                AddLog(log);
            }
        }
    }
}
