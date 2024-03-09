using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Controls;
using Windows.ApplicationModel.UserDataTasks.DataProvider;
using Windows.Storage;
using Winestro_A.Attributes;
using Winestro_A.Controls;
using Winestro_A.Enums;
using Winestro_A.Structures;

namespace Winestro_A.Services;
public class IntegratedConsoleService
{
    public static Stack<string> СonsolePromts = new();
    public static ObservableCollection<ConsoleMessageControl> ConsoleHistory { get; private set; } = new();
    private static readonly ObservableCollection<Func<ConsoleCommandContext, ConsoleCommandResult>> CommandsList = new()
    {
        Test1,
        Test0,
        Test2,
        Log,
        ShowSettings,
        CreateSetting,
        RemoveSetting,
    };

    public static bool TryRun(string promt, out ConsoleCommandResult result)
    {
        ConsoleHistory.Add(new ConsoleMessageControl() { Type = Enums.ConsoleMessageTypes.Command, Text = promt });
        result = new ConsoleCommandResult() { OutMessage = "Unhandled exception", Success = false, Type = Enums.ConsoleMessageTypes.Fail };

        Func<ConsoleCommandContext, ConsoleCommandResult>? method;
        ConsoleCommandContext? ctx;

        var r = TryGetAppropriateCommand(promt, out method, out ctx);
        if (r.Success)
        {
            result = method.Invoke((ConsoleCommandContext)ctx);
        }
        else
        {
            result.Success = r.Success;
            result.OutMessage = r.Message;
            result.Type = ConsoleMessageTypes.Fail;
        }

        ConsoleHistory.Add(new ConsoleMessageControl() { Type = result.Type, Text = result.OutMessage ??= string.Empty });
        return result.Success;
    }

    private static bool IsWord(string promt)
    {
        return Regex.IsMatch(promt, @"^[a-zA-Z]+$");
    }
    private static bool IsKwarg(string promt, out string? key, out string? value)
    {
        var result = Regex.IsMatch(promt, "^[a-zA-Z0-9]+={1}.+$");

        if (result)
        {
            var parts = promt.Split("=");
            key = parts[0];
            value = parts[1];
        }
        else
        {
            key = null;
            value = null;
        }

        return result;
    }
    private static ResultManifest TryGetAppropriateCommand(string promt, out Func<ConsoleCommandContext, ConsoleCommandResult>? method, out ConsoleCommandContext? ctx)
    {
        method = null;
        ctx = null;
        ICCommandAttribute? cattr = null;
        if (string.IsNullOrWhiteSpace(promt))
        {
            return new(false, "The command promt is empty");
        }

        var splitted_promt = promt.Split(' ');
        var depth = 1;
        var finalName = string.Empty;

        while (depth <= splitted_promt.Length)
        {
            var name = string.Join(" ", splitted_promt[..depth]);
            if (TryGetCommandByName(name, out var attr).Success)
            {
                finalName = name;
                cattr = attr;
                depth++;
            }
            else
            {
                break;
            }
        }

        if (finalName == string.Empty)
        {
            return new(false, "Cannot find command");
        }

        var overloadSearchResult = TryGetAppropriateCommandOverload(finalName, splitted_promt[(depth-1)..], out method, out ctx);
        if (overloadSearchResult.Success)
        {
            return new(true);
        }
        else
        {
            return new(false, overloadSearchResult.Message);
        }
    }
    private static ResultManifest TryGetCommandByName(string name, out ICCommandAttribute? cattr)
    {
        cattr = null;

        foreach (var command in CommandsList)
        {
            var attributes = command.GetMethodInfo().GetCustomAttributes();
            foreach (Attribute attr in attributes)
            {
                if (attr is ICCommandAttribute tcattr)
                {
                    if (tcattr.IsNameEqual(name))
                    {
                        cattr = tcattr;
                        return new(true);
                    }
                }
            }
        }

        return new(false, $"Cannot find command [{name}]");
    }
    private static ResultManifest TryGetAppropriateCommandOverload(string name, string[] args_query, out Func<ConsoleCommandContext, ConsoleCommandResult>? method, out ConsoleCommandContext? ctx)
    {
        method = null;
        ctx = null;

        List<string> Args = new();
        Dictionary<string, string> Kwargs = new();

        foreach (var arg in args_query)
        {
            if (IsKwarg(arg, out var vkey, out var value))
            {
                Kwargs.Add(vkey, value);
            }
            else
            {
                Args.Add(arg);
            }
        }

        Dictionary<ICCommandAttribute, Func<ConsoleCommandContext, ConsoleCommandResult>> Overloads = new();

        foreach (var command in CommandsList)
        {
            var attributes = command.GetMethodInfo().GetCustomAttributes();
            foreach (Attribute attr in attributes)
            {
                if (attr is ICCommandAttribute tcattr)
                {
                    if (tcattr.IsNameEqual(name))
                    {
                        Overloads.Add(tcattr, command);
                    }
                }
            }
        }

        ICCommandAttribute? key = null;

        if (Overloads.Keys.Count == 1)
        {
            key = Overloads.Keys.First();
        }
        else
        {
            foreach (var att in Overloads.Keys)
            {
                if (att.RequiredArgs == Args.Count)
                {
                    key = att;
                }
            }
        }

        if (key != null)
        {
            if (key.RequiredArgs <= Args.Count)
            {
                if (key.KwargsKeys == null && Kwargs.Keys.Count != 0)
                {
                    return new(false, $"Keyword argument exception: this command has no keyword arguments");
                }
                foreach (var kwarg in Kwargs.Keys)
                {
                    if (!key.KwargsKeys.Contains(kwarg))
                    {
                        return new(false, $"Keyword argument exception: unexpected keyword: [{kwarg}]");
                    }
                }

                method = Overloads[key];
                ctx = new() { Args=Args, Kwargs=Kwargs };
                return new(true);
            }
            else
            {
                return new(false, $"Arguments exception: expected {key.RequiredArgs} positional arguments, but {Args.Count} were given");
            }
        }

        return new(false, $"Cannot find appropriate overload for [{name}]");
    }
    
    // HERE COMMANDS GO

    // EVERY COMMAND HAS TO BE LIKE
    // [ICCommand("CommandName", [Aliases: string[]], [nArgs: int], [KwargsKeys: string[]])]
    // ... static ConsoleCommandResult MethodName(ConsoleCommandContext) { }

    [ICCommand("test", RequiredArgs = 1)]
    private static ConsoleCommandResult Test1(ConsoleCommandContext ctx)
    {
        return new ConsoleCommandResult()
        {
            Success = true,
            Type = Enums.ConsoleMessageTypes.Ok,
            OutMessage = $"Hello, world! 1"
        };
    }

    [ICCommand("test")]
    private static ConsoleCommandResult Test0(ConsoleCommandContext ctx)
    {
        return new ConsoleCommandResult()
        {
            Success = true,
            Type = Enums.ConsoleMessageTypes.Ok,
            OutMessage = $"Hello, world! 0"
        };
    }

    [ICCommand("test", RequiredArgs = 2)]
    private static ConsoleCommandResult Test2(ConsoleCommandContext ctx)
    {
        return new ConsoleCommandResult()
        {
            Success = true,
            Type = Enums.ConsoleMessageTypes.Ok,
            OutMessage = $"Hello, world! 2"
        };
    }

    [ICCommand("log", RequiredArgs = 1, KwargsKeys = new string[]{"type", "meta"})]
    private static ConsoleCommandResult Log(ConsoleCommandContext ctx)
    {
        LogMessageTypes? type = null;
        LogMessageMetaTypes? meta = null;

        if (ctx.Kwargs.ContainsKey("type"))
        {
            switch (ctx.Kwargs["type"])
            {
                case "info":
                    type = LogMessageTypes.Info;
                    break;
                case "warning":
                    type = LogMessageTypes.Warning;
                    break;
                case "error":
                    type = LogMessageTypes.Error;
                    break;
                default:
                    return new ConsoleCommandResult()
                    {
                        Success = false,
                        Type = Enums.ConsoleMessageTypes.Fail,
                        OutMessage = "Wrong type argument. Available args: [info, warning, error]"
                    };
            }
        }
        if (ctx.Kwargs.ContainsKey("meta"))
        {
            switch (ctx.Kwargs["meta"])
            {
                case "default":
                    meta = LogMessageMetaTypes.Default;
                    break;
                case "music":
                    meta = LogMessageMetaTypes.Music;
                    break;
                case "misc":
                    meta = LogMessageMetaTypes.Misc;
                    break;
                case "debug":
                    meta = LogMessageMetaTypes.Debug;
                    break;
                default:
                    return new ConsoleCommandResult()
                    {
                        Success = false,
                        Type = Enums.ConsoleMessageTypes.Fail,
                        OutMessage = "Wrong meta-type argument. Available args: [music, misc, debug]"
                    };
            }
        }

        switch (type)
        {
            case LogMessageTypes.Warning:
                LogService.Warning(string.Join(" ", ctx.Args), meta ??= Enums.LogMessageMetaTypes.Debug);
                return new ConsoleCommandResult()
                {
                    Success = true,
                    Type = Enums.ConsoleMessageTypes.Ok,
                    OutMessage = $"Reported warning to {meta} log tab"
                };
            case LogMessageTypes.Error:
                LogService.Error(string.Join(" ", ctx.Args), meta ??= Enums.LogMessageMetaTypes.Debug);
                return new ConsoleCommandResult()
                {
                    Success = true,
                    Type = Enums.ConsoleMessageTypes.Ok,
                    OutMessage = $"Reported error to {meta} log tab"
                };
            default:
                LogService.Log(string.Join(" ", ctx.Args), meta ??= Enums.LogMessageMetaTypes.Debug);
                return new ConsoleCommandResult()
                {
                    Success = true,
                    Type = Enums.ConsoleMessageTypes.Ok,
                    OutMessage = $"Logged to {meta} log tab"
                };
        }
    }

    [ICCommand("conf")]
    private static ConsoleCommandResult ShowSettings(ConsoleCommandContext ctx)
    {
        var count = ApplicationData.Current.LocalSettings.Values.Keys.Count();
        var ret = count > 0 ? $"Application settings: [{count}]" : "No settings detected";
        var counter = 0;
        foreach (var key in ApplicationData.Current.LocalSettings.Values.Keys)
        {
            ret += $"\n{++counter}. {key}={ApplicationData.Current.LocalSettings.Values[key]}";
        }
        return new ConsoleCommandResult()
        {
            OutMessage = ret,
            Success = true,
            Type = Enums.ConsoleMessageTypes.Info
        };
    }

    [ICCommand("conf add", RequiredArgs = 2)]
    private static ConsoleCommandResult CreateSetting(ConsoleCommandContext ctx)
    {
        var ok = ConfigService.Add(ctx.Args[0], ctx.Args[1]);
        return new ConsoleCommandResult()
        {
            Success = ok,
            Type = ok? Enums.ConsoleMessageTypes.Ok : Enums.ConsoleMessageTypes.Fail,
            OutMessage = ok? $"Created setting [{ctx.Args[0]}={ctx.Args[1]}]" : $"Failed creating setting [{ctx.Args[0]}={ctx.Args[1]}]"
        };
    }

    [ICCommand("conf del", RequiredArgs = 1)]
    private static ConsoleCommandResult RemoveSetting(ConsoleCommandContext ctx)
    {
        var ok = ConfigService.Delete(ctx.Args[0]);
        return new ConsoleCommandResult()
        {
            Success = ok,
            Type = ok? Enums.ConsoleMessageTypes.Ok : Enums.ConsoleMessageTypes.Fail,
            OutMessage = ok? $"Removed setting [{ctx.Args[0]}]" : $"Failed removing setting [{ctx.Args[0]}]"
        };
    }
}
