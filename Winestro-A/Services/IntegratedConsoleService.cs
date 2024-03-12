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
using Winestro_A.Discord;
using Winestro_A.Enums;
using Winestro_A.Structures;
using Winestro_A.Youtube;

namespace Winestro_A.Services;
public static class IntegratedConsoleService
{
    public static Stack<string> СonsolePromts = new();
    public static ObservableCollection<ConsoleMessageControl> ConsoleHistory { get; private set; } = new();
    private static readonly ObservableCollection<Func<ConsoleCommandContext, Task<ConsoleCommandResult>>> CommandsList = new();

    public static void Init()
    {
        var methods = typeof(IntegratedConsoleService).GetMethods(BindingFlags.Static | BindingFlags.NonPublic);
        var counter = 0;

        foreach (var method in methods)
        {
            var found = false;
            ConsoleCommandAttribute? iccattr = null;
            foreach (var attrib in method.GetCustomAttributes())
            {
                if (attrib is ConsoleCommandAttribute _iccattr)
                {
                    found = true; 
                    iccattr = _iccattr;
                    break;
                }
            }

            if (found)
            {
                CommandsList.Add(method.CreateDelegate<Func<ConsoleCommandContext, Task<ConsoleCommandResult>>>());
                LogService.Log($"Console command [{iccattr.Name}] compiled", Enums.LogMessageMetaTypes.Debug);
                counter++;
            }
        }

        LogService.Log($"All console commands successfully compiled: {counter}");
    }

    public static bool TryRun(string promt, out ConsoleCommandResult result)
    {
        ConsoleHistory.Add(new ConsoleMessageControl() { Type = Enums.ConsoleMessageTypes.Command, Text = promt });
        result = new ConsoleCommandResult() { OutMessage = "Unhandled exception", Success = false, Type = Enums.ConsoleMessageTypes.Fail };

        Func<ConsoleCommandContext, Task<ConsoleCommandResult>>? method;
        ConsoleCommandContext? ctx;

        var r = TryGetAppropriateCommand(promt, out method, out ctx);
        if (r.Success)
        {
            result = Task.Run(async() => await method.Invoke((ConsoleCommandContext)ctx)).Result;
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
    private static ResultManifest TryGetAppropriateCommand(string promt, out Func<ConsoleCommandContext, Task<ConsoleCommandResult>>? method, out ConsoleCommandContext? ctx)
    {
        method = null;
        ctx = null;
        ConsoleCommandAttribute? cattr = null;
        if (string.IsNullOrWhiteSpace(promt))
        {
            return new(false, "The command promt is empty");
        }

        var splitted_promt = promt.Split(' ');
        var depth = 1;
        var finalName = string.Empty;
        var finalArgs = new string[0];

        while (depth <= splitted_promt.Length)
        {
            var name = string.Join(" ", splitted_promt[..depth]);
            if (TryGetCommandByName(name, out var attr).Success)
            {
                finalName = name;
                finalArgs = splitted_promt[depth..];
                cattr = attr;
            }
            depth++;
        }

        if (finalName == string.Empty)
        {
            return new(false, "Cannot find command");
        }

        var overloadSearchResult = TryGetAppropriateCommandOverload(finalName, finalArgs, out method, out ctx);
        if (overloadSearchResult.Success)
        {
            return new(true);
        }
        else
        {
            return new(false, overloadSearchResult.Message);
        }
    }
    private static ResultManifest TryGetCommandByName(string name, out ConsoleCommandAttribute? cattr)
    {
        cattr = null;

        foreach (var command in CommandsList)
        {
            var attributes = command.GetMethodInfo().GetCustomAttributes();
            foreach (Attribute attr in attributes)
            {
                if (attr is ConsoleCommandAttribute tcattr)
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
    private static ResultManifest TryGetAppropriateCommandOverload(string name, string[] args_query, out Func<ConsoleCommandContext, Task<ConsoleCommandResult>>? method, out ConsoleCommandContext? ctx)
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

        Dictionary<ConsoleCommandAttribute, Func<ConsoleCommandContext, Task<ConsoleCommandResult>>> Overloads = new();

        foreach (var command in CommandsList)
        {
            var attributes = command.GetMethodInfo().GetCustomAttributes();
            foreach (Attribute attr in attributes)
            {
                if (attr is ConsoleCommandAttribute tcattr)
                {
                    if (tcattr.IsNameEqual(name))
                    {
                        Overloads.Add(tcattr, command);
                    }
                }
            }
        }

        ConsoleCommandAttribute? key = null;

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

    [ConsoleCommand("test")]
    private static async Task<ConsoleCommandResult> Test(ConsoleCommandContext ctx)
    {
        return new ConsoleCommandResult($"Hello, world!");
    }

    [ConsoleCommand("log", RequiredArgs = 1, KwargsKeys = new string[]{"type", "meta"})]
    private static async Task<ConsoleCommandResult> Log(ConsoleCommandContext ctx)
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

    [ConsoleCommand("conf")]
    private static async Task<ConsoleCommandResult> ShowSettings(ConsoleCommandContext ctx)
    {
        var count = ApplicationData.Current.LocalSettings.Values.Keys.Count();
        var ret = count > 0 ? $"Application settings: [{count}]" : "No settings detected";
        var counter = 0;
        foreach (var key in ApplicationData.Current.LocalSettings.Values.Keys)
        {
            ret += $"\n{++counter}. {key}={ApplicationData.Current.LocalSettings.Values[key]}";
        }
        return new ConsoleCommandResult(ret, true, ConsoleMessageTypes.Info);
    }

    [ConsoleCommand("conf add", RequiredArgs = 2)]
    private static async Task<ConsoleCommandResult> CreateSetting(ConsoleCommandContext ctx)
    {
        var ok = ConfigService.Add(ctx.Args[0], ctx.Args[1]);
        return new ConsoleCommandResult(
            ok ? $"Created setting [{ctx.Args[0]}={ctx.Args[1]}]" : $"Failed creating setting [{ctx.Args[0]}={ctx.Args[1]}]",
            ok,
            ok ? Enums.ConsoleMessageTypes.Ok : Enums.ConsoleMessageTypes.Fail
        );
    }

    [ConsoleCommand("conf del", RequiredArgs = 1)]
    private static async Task<ConsoleCommandResult> RemoveSetting(ConsoleCommandContext ctx)
    {
        var ok = ConfigService.Delete(ctx.Args[0]);
        return new ConsoleCommandResult(
            ok ? $"Removed setting [{ctx.Args[0]}]" : $"Failed removing setting [{ctx.Args[0]}]",
            ok,
            ok ? Enums.ConsoleMessageTypes.Ok : Enums.ConsoleMessageTypes.Fail
        );
    }

    [ConsoleCommand("discord bot run")]
    private static async Task<ConsoleCommandResult> BotRun(ConsoleCommandContext ctx)
    {
        await DiscordBotService.Run();
        return new ConsoleCommandResult($"Launching bot...");
    }

    [ConsoleCommand("discord bot stop")]
    private static async Task<ConsoleCommandResult> BotStop(ConsoleCommandContext ctx)
    {
        await DiscordBotService.Stop();
        return new ConsoleCommandResult($"Stopping bot...");
    }

    [ConsoleCommand("discord bot commands reg")]
    private static async Task<ConsoleCommandResult> BotRegisterSlashCommands(ConsoleCommandContext ctx)
    {
        //await DiscordBotService.RegisterGlobalSlashCommands();
        return new("Console commands have been registered");
    }

    [ConsoleCommand("discord bot commands reg test")]
    private static async Task<ConsoleCommandResult> BotRegisterTestSlashCommands(ConsoleCommandContext ctx)
    {
        if (await DiscordBotService.RegisterTestCommandsAsync())
        {
            return new("Console commands have been registered");
        }
        else
        {
            return new("Exception raised when registering *TEST* slash commands");
        }
    }

    [ConsoleCommand("sudo discord bot slash delete all")]
    private static async Task<ConsoleCommandResult> BotDeleteAllGlobalSlashCommands(ConsoleCommandContext ctx)
    {
        //await DiscordBotService.DeleteSlashCommands();
        return new("Global slash commands will be deleted in ~1 hour");
    }    
}
