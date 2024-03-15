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
    public static ObservableCollection<LogMessageControl> MainMessages { get; private set; } = new();
    public static ObservableCollection<LogMessageControl> InfoMessages { get; private set; } = new();
    public static ObservableCollection<LogMessageControl> WarningMessages { get; private set; } = new();
    public static ObservableCollection<LogMessageControl> ErrorMessages { get; private set; } = new();

    public static ObservableCollection<LogMessageControl> MusicMessages { get; private set; } = new();
    public static ObservableCollection<LogMessageControl> MiscMessages { get; private set; } = new();
    public static ObservableCollection<LogMessageControl> DebugMessages { get; private set; } = new();

    public static bool UpdateRequired = false;

    public static void Log(string msg, LogMeta meta = LogMeta.Default)
    {
        TryLog(msg, LogSeverity.Info, meta);
    }
    public static void Log(object obj, LogMeta meta = LogMeta.Default)
    {
        TryLog(obj.ToString(), LogSeverity.Info, meta);
    }
    public static void Warning(string msg, LogMeta meta = LogMeta.Default)
    {
        TryLog(msg, LogSeverity.Warning, meta);
    }
    public static void Warning(object obj, LogMeta meta = LogMeta.Default)
    {
        TryLog(obj.ToString(), LogSeverity.Warning, meta);
    }
    public static void Error(string msg, LogMeta meta = LogMeta.Default)
    {
        TryLog(msg, LogSeverity.Error, meta);
    }
    public static void Error(object obj, LogMeta meta = LogMeta.Default)
    {
        TryLog(obj.ToString(), LogSeverity.Error, meta);
    }
    public static void ForceUpdateControlsCollections()
    {
        try
        {
            _UpdateControlsCollections_();
        }
        catch { }
    }

    private static void TryLog(string msg, LogSeverity type, LogMeta meta)
    {
        if (string.IsNullOrWhiteSpace(msg)) return;
        try
        {
            logs.Add(new LogMessageManifest() { Text=msg, Type=type, Meta=meta, Time=TimeHelper.GetNowString(), Id=(ulong)logs.Count });
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

        if (log.Meta == LogMeta.Default)
        {
            LogMessageControl lmcmain = new()
            {
                Text = log.Text,
                Type = log.Type,
                Time = log.Time,
                Meta = log.Meta,
                Id = log.Id,
            };
            MainMessages.Add(lmcmain);
        }

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
            case LogSeverity.Warning:
                WarningMessages.Add(lmce);
                break;
            case LogSeverity.Error:
                ErrorMessages.Add(lmce);
                break;
            default:
                if (log.Meta == LogMeta.Default)
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
            case LogMeta.Music:
                MusicMessages.Add(lmcmeta);
                break;
            case LogMeta.Misc:
                MiscMessages.Add(lmcmeta);
                break;
            case LogMeta.Debug:
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
