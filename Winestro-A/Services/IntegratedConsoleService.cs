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
        Test,
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
        var validationResult = IsCommandValid(promt, out ctx, out method);
        if (!validationResult.Success)
        {
            ConsoleHistory.Add(new ConsoleMessageControl() { Type = ConsoleMessageTypes.Fail, Text = validationResult.ErrorMessage });
            return false;
        }

        var r = method?.Invoke(ctx.Value);
        result = r.Value;

        ConsoleHistory.Add(new ConsoleMessageControl() { Type = result.Type, Text = result.OutMessage ??= string.Empty });
        return result.Success;
    }
    private static bool TryParse(string[] promt, out ConsoleCommandContext? ctx, out string? errorMessage)
    {
        ctx = null;
        errorMessage = null;

        List<string> Args = new();
        Dictionary<string, string> Kwargs = new();

        foreach (var arg in promt)
        {
            if (IsWord(arg))
            {
                Args.Add(arg);
            }
            else if (IsKwarg(arg, out var key, out var value))
            {
                Kwargs.Add(key, value);
            }
            else
            {
                errorMessage = $"Unexpected argument: {arg}";
                return false;
            }
        }

        ctx = new ConsoleCommandContext() { Args=Args, Kwargs=Kwargs };

        return true;
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
    private static bool IsCommandExist(string promt, out ICCommandAttribute? attr, out Func<ConsoleCommandContext, ConsoleCommandResult>? method)
    {
        attr = null;
        method = null;

        var splitted = promt.Split(' ');

        for (var name_len = splitted.Length; name_len > 0; name_len--)
        {
            var test_name = splitted[..name_len];
            if (IsCommandExistFlat(string.Join(" ", test_name), out var cattr, out method))
            {
                attr = cattr;
                return true;
            }

        }

        return false;
    }
    private static bool IsCommandExistFlat(string promt, out ICCommandAttribute? cmdattr, out Func<ConsoleCommandContext, ConsoleCommandResult>? method)
    {
        cmdattr = null;
        method = null;
        foreach (var command in CommandsList)
        {
            var attributes = command.GetMethodInfo().GetCustomAttributes();

            foreach (Attribute attr in attributes)
            {
                if (attr is ICCommandAttribute cattr)
                {
                    if (cattr.IsNameEqual(promt))
                    {
                        cmdattr = cattr;
                        method = command;
                        return true;
                    }
                }
            }
        }

        return false;
    }
    private static ConsoleCommandValidationInfo IsCommandValid(string promt, out ConsoleCommandContext? promt_ctx, out Func<ConsoleCommandContext, ConsoleCommandResult>? method)
    {
        method = null;
        promt_ctx = null;

        if (IsCommandExist(promt, out var attr, out var tmethod))
        {
            var arguments = promt.Split(' ')[attr.Name.Split(' ').Length..];
            if (TryParse(arguments, out var tctx, out var errorMsg))
            {
                if (tctx.Value.Args.Count() < attr.RequiredArgs)
                {
                    return new() { 
                        Success = false, 
                        ErrorMessage=$"Args error: expected {attr.RequiredArgs} positional arguments, but {tctx.Value.Args.Count()} were given" 
                    };
                }
                
                foreach (var kwarg in tctx.Value.Kwargs.Keys)
                {
                    if (!attr.KwargsKeys.Contains(kwarg))
                    {
                        return new()
                        {
                            Success = false,
                            ErrorMessage = $"Key-word args error: unexpected key-word argument: {kwarg}"
                        };
                    }
                }

                promt_ctx = tctx;
                method = tmethod;
            }
            else
            {
                return new() {
                    Success=false,
                    ErrorMessage=errorMsg??"Unhandled exception"
                };
            }
        }
        
        return new() {
            Success = true
        };
    }

    // HERE COMMANDS GO

    // EVERY COMMAND HAS TO BE LIKE
    // [ICCommand("CommandName", [Aliases: string[]], [nArgs: int], [KwargsKeys: string[]])]
    // ... static ConsoleCommandResult MethodName(ConsoleCommandContext) { }

    [ICCommand("test")]
    private static ConsoleCommandResult Test(ConsoleCommandContext ctx)
    {
        return new ConsoleCommandResult()
        {
            Success = true,
            Type = Enums.ConsoleMessageTypes.Ok,
            OutMessage = $"Hello, world!"
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

    [ICCommand("setadd", RequiredArgs = 2)]
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

    [ICCommand("setdel", RequiredArgs = 1)]
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

    [ICCommand("sets")]
    private static ConsoleCommandResult ShowSettings(ConsoleCommandContext ctx)
    {
        var count = ApplicationData.Current.LocalSettings.Values.Keys.Count();
        var ret = count > 0? $"Application settings: [{count}]" : "No settings detected";
        var counter = 0;
        foreach (var key in ApplicationData.Current.LocalSettings.Values.Keys)
        {
            ret += $"\n{++counter}. {key}={ApplicationData.Current.LocalSettings.Values[key]}";
        }
        return new ConsoleCommandResult()
        {
            OutMessage=ret,
            Success=true,
            Type=Enums.ConsoleMessageTypes.Info
        };
    }
}
