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
                LogService.Log($"Console command [{iccattr.Name}] compiled", Enums.LogMeta.Debug);
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
            try
            {
                result = Task.Run(async() => await method.Invoke((ConsoleCommandContext)ctx)).Result;
            }
            catch (Exception ex)
            {
                result = new($"Unhandled exception raised when invoking {ctx.Value.Name}: {ex}", false);
            }
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
                ctx = new(name, Args, Kwargs);
                return new(true);
            }
            else
            {
                return new(false, $"Arguments exception: expected {key.RequiredArgs} positional arguments, but {Args.Count} were given");
            }
        }

        return new(false, $"Cannot find appropriate overload for [{name}]");
    }
    private static List<ConsoleCommandAttribute> GetCommandsAttributes()
    {
        var list = new List<ConsoleCommandAttribute>();
        foreach (var command in CommandsList)
        {
            var attributes = command.GetMethodInfo().GetCustomAttributes();
            foreach (Attribute attr in attributes)
            {
                if (attr is ConsoleCommandAttribute tcattr)
                {
                    list.Add(tcattr);
                }
            }
        }
        return list;
    }

    // HERE COMMANDS GO

    // EVERY COMMAND HAS TO BE LIKE
    // [ICCommand("CommandName", [Aliases: string[]], [nArgs: int], [KwargsKeys: string[]])]
    // ... static ConsoleCommandResult MethodName(ConsoleCommandContext) { }

    [ConsoleCommand("test", Description = "Test command for testing console")]
    private static async Task<ConsoleCommandResult> Test(ConsoleCommandContext ctx)
    {
        return new ConsoleCommandResult($"Hello, world!");
    }

    [ConsoleCommand("help", Description = "Lists all console commands")]
    private static async Task<ConsoleCommandResult> Help(ConsoleCommandContext ctx)
    {
        var attrs = GetCommandsAttributes();
        var output = $"Console commands ({attrs.Count})";

        for (var i = 0; i < attrs.Count; i++)
        {
            output += $"\n{i+1}. [{attrs[i].Name}] -> {attrs[i].Description ?? "No description"}";
        }

        return new(output, true, ConsoleMessageTypes.Info);
    }

    [ConsoleCommand("log", RequiredArgs = 1, KwargsKeys = new string[]{"type", "meta"}, Description = "Log to log panel. Syntax: log <word1> ... [type=info|warning|error] [meta=music|misc|debug]")]
    private static async Task<ConsoleCommandResult> Log(ConsoleCommandContext ctx)
    {
        LogSeverity? type = null;
        LogMeta? meta = null;

        if (ctx.Kwargs.ContainsKey("type"))
        {
            switch (ctx.Kwargs["type"])
            {
                case "info":
                    type = LogSeverity.Info;
                    break;
                case "warning":
                    type = LogSeverity.Warning;
                    break;
                case "error":
                    type = LogSeverity.Error;
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
                    meta = LogMeta.Default;
                    break;
                case "music":
                    meta = LogMeta.Music;
                    break;
                case "misc":
                    meta = LogMeta.Misc;
                    break;
                case "debug":
                    meta = LogMeta.Debug;
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
            case LogSeverity.Warning:
                LogService.Warning(string.Join(" ", ctx.Args), meta ??= Enums.LogMeta.Debug);
                return new ConsoleCommandResult()
                {
                    Success = true,
                    Type = Enums.ConsoleMessageTypes.Ok,
                    OutMessage = $"Reported warning to {meta} log tab"
                };
            case LogSeverity.Error:
                LogService.Error(string.Join(" ", ctx.Args), meta ??= Enums.LogMeta.Debug);
                return new ConsoleCommandResult()
                {
                    Success = true,
                    Type = Enums.ConsoleMessageTypes.Ok,
                    OutMessage = $"Reported error to {meta} log tab"
                };
            default:
                LogService.Log(string.Join(" ", ctx.Args), meta ??= Enums.LogMeta.Debug);
                return new ConsoleCommandResult()
                {
                    Success = true,
                    Type = Enums.ConsoleMessageTypes.Ok,
                    OutMessage = $"Logged to {meta} log tab"
                };
        }
    }

    [ConsoleCommand("conf", Description = "Lists all config settngs")]
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

    [ConsoleCommand("conf add", RequiredArgs = 2, Description = "Add setting to config. Syntax: conf add <key> <value>")]
    private static async Task<ConsoleCommandResult> CreateSetting(ConsoleCommandContext ctx)
    {
        var ok = ConfigService.Add(ctx.Args[0], ctx.Args[1]);
        return new ConsoleCommandResult(
            ok ? $"Created setting [{ctx.Args[0]}={ctx.Args[1]}]" : $"Failed creating setting [{ctx.Args[0]}={ctx.Args[1]}]",
            ok,
            ok ? Enums.ConsoleMessageTypes.Ok : Enums.ConsoleMessageTypes.Fail
        );
    }

    [ConsoleCommand("conf del", RequiredArgs = 1, Description = "Removed setting from config. Syntax: conf del <key>")]
    private static async Task<ConsoleCommandResult> RemoveSetting(ConsoleCommandContext ctx)
    {
        var ok = ConfigService.Delete(ctx.Args[0]);
        return new ConsoleCommandResult(
            ok ? $"Removed setting [{ctx.Args[0]}]" : $"Failed removing setting [{ctx.Args[0]}]",
            ok,
            ok ? Enums.ConsoleMessageTypes.Ok : Enums.ConsoleMessageTypes.Fail
        );
    }

    [ConsoleCommand("discord bot run", Description = "Runs discord bot")]
    private static async Task<ConsoleCommandResult> BotRun(ConsoleCommandContext ctx)
    {
        await DiscordBotService.Run();
        return new ConsoleCommandResult($"Launching bot...");
    }

    [ConsoleCommand("discord bot stop", Description = "Stops discord bot")]
    private static async Task<ConsoleCommandResult> BotStop(ConsoleCommandContext ctx)
    {
        await DiscordBotService.Stop();
        return new ConsoleCommandResult($"Stopping bot...");
    }

    [ConsoleCommand("discord bot commands reg", Description = "Registers all global discord commands")]
    private static async Task<ConsoleCommandResult> BotRegisterSlashCommands(ConsoleCommandContext ctx)
    {
        //await DiscordBotService.RegisterGlobalSlashCommands();
        return new("Console commands have been registered");
    }

    [ConsoleCommand("sudo discord bot slash delete all", Description = "Deletes ALL global commands from the discord")]
    private static async Task<ConsoleCommandResult> BotDeleteAllGlobalSlashCommands(ConsoleCommandContext ctx)
    {
        //await DiscordBotService.DeleteSlashCommands();
        return new("Global slash commands will be deleted in ~1 hour");
    }

    [ConsoleCommand("discord commands reg test", Description = "Registers all test discord commands to test server")]
    private static async Task<ConsoleCommandResult> BotRegisterSlashCommandsFromTestGuild(ConsoleCommandContext ctx)
    {
        var res = await DiscordBotService.TryRegisterAllCommandsToGuildAsync();
        if (res.Success)
        {
            return new("Console commands have been registered to test guild");
        }
        else
        {
            return new("Exception raised when registering *TEST* slash commands: " + res.Message, false);
        }
    }

    [ConsoleCommand("discord commands unreg test", Description = "Registers all test discord commands to test server")]
    private static async Task<ConsoleCommandResult> BotUnregisterSlashCommandsFromTestGuild(ConsoleCommandContext ctx)
    {
        var res = await DiscordBotService.TryUnregisterAllCommandsFromGuildAsync();
        if (res.Success)
        {
            return new("Console commands have been unregistered from test guild");
        }
        else
        {
            return new("Exception raised when registering *TEST* slash commands: " + res.Message, false);
        }
    }

     
}
