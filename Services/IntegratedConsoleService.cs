using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Controls;
using Windows.ApplicationModel.UserDataTasks.DataProvider;
using Winestro_A.Attributes;
using Winestro_A.Controls;
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
        CreateSetting,
    };

    public static bool TryRun(string promt, out ConsoleCommandResult result)
    {
        ConsoleHistory.Add(new ConsoleMessageControl() { Type = Enums.ConsoleMessageTypes.Command, Text = promt });
        result = new ConsoleCommandResult() { OutMessage = "Unhandled exception", Success=false, Type=Enums.ConsoleMessageTypes.Fail };

        var parseResult = TryParse(promt, out var cmd, out var error);

        if (!parseResult)
        {
            result = new ConsoleCommandResult() { OutMessage = (error ??= "Command parsing unhandled error"), Success=false, Type = Enums.ConsoleMessageTypes.Fail };
        }
        else
        {
            if (cmd != null)
            {
                var input = cmd.Value;
                input.Name = input.Name.ToLower();
                Func<ConsoleCommandContext, ConsoleCommandResult>? commandMethod = null;
                ICCommandAttribute? commandInfo = null;

                foreach (var command in CommandsList)
                {
                    var attributes = command.GetMethodInfo().GetCustomAttributes();

                    var foundCommand = false;
                    foreach (Attribute attr in attributes)
                    {
                        if (attr is ICCommandAttribute cattr)
                        {
                            List<string> names = new List<string>{ cattr.Name.ToLower() };
                            if (cattr.Aliases != null)
                            {
                                foreach (var alias in cattr.Aliases)
                                {
                                    names.Add(alias.ToLower());
                                }
                            }
                            if (names.Contains(input.Name))
                            {
                                foundCommand = true;
                                commandMethod = command;
                                commandInfo = cattr;
                                break;
                            }
                        }
                    }

                    if (foundCommand)
                    {
                        break;
                    }
                }

                if (commandMethod == null)
                {
                    result = new ConsoleCommandResult() { OutMessage=$"Command [{input.Name}] not found", Success = false, Type = Enums.ConsoleMessageTypes.Fail };
                }
                else
                {
                    if (input.Args.Count() != commandInfo.nArgs && commandInfo.nArgs != -1)
                    {
                        result = new ConsoleCommandResult() { OutMessage = $"Arguments exception. Expected {commandInfo.nArgs} positional arguments, but {input.Args.Count()} were given", Success=false, Type=Enums.ConsoleMessageTypes.Fail };
                    }
                    else
                    {
                        if (commandInfo.nArgs == -1)
                        {
                            var logarg = string.Join(" ", input.Args);
                            input.Args.Clear();
                            input.Args.Add(logarg);
                        }

                        var goodKwargs = true;
                        var strangeKey = "What?";
                        foreach (var key in input.Kwargs.Keys)
                        {
                            if (commandInfo.KwargsKeys == null || !commandInfo.KwargsKeys.Contains(key))
                            {
                                goodKwargs = false;
                                strangeKey = key;
                            }
                        }

                        if (!goodKwargs)
                        {
                            result = new ConsoleCommandResult() { OutMessage = $"Keyword arguments exception. Unexpected argument key: [{strangeKey}]", Success=false, Type=Enums.ConsoleMessageTypes.Fail };
                        }
                        else
                        {
                            if (commandMethod != null)
                            {
                                result = commandMethod(new ConsoleCommandContext() { Args=input.Args.ToArray(), Kwargs=input.Kwargs });
                            }
                        }
                    }
                }
            }
        }


        ConsoleHistory.Add(new ConsoleMessageControl() { Type = result.Type, Text = result.OutMessage ??= string.Empty });
        return result.Success;
    }
    private static bool TryParse(string promt, out ConsoleCommand? cmd, out string? errorMesage)
    {
        // Заранее присвоим cmd и errorMessage null 
        cmd = null;
        errorMesage = null;

        // Если строка пустая или состоит только из пробелов, то вернем 0
        if (String.IsNullOrWhiteSpace(promt))
        {
            errorMesage = "Empty or blank command.\nHow the fu...??";
            return false;
        }

        // Разобьем всю строку на куски
        var parts = promt.Split(' ');

        // Проверим является ли первое слово названием команды (только латинские буквы)
        if (!IsWord(parts[0]))
        {
            errorMesage = $"Wrong command name [{parts[0]}]";
            return false;
        }

        // Если является, создадим пустую команду с этим именем
        cmd = new ConsoleCommand()
        {
            Name = parts[0],
            Args = new List<string>(),
            Kwargs = new Dictionary<string, string>()
        };

        // Пройдем по всем оставшимся частям
        // Проверим чем он является и засунем в нужный контейнер
        for (var i = 1; i < parts.Length; i++)
        {
            if (IsKwarg(parts[i], out var key, out var value))
            {
                cmd.Value.Kwargs.Add(key, value);
            }
            else
            {
                cmd.Value.Args.Add(parts[i]);
            }
        }

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

    [ICCommand("log", nArgs = -1)]
    private static ConsoleCommandResult Log(ConsoleCommandContext ctx)
    {
        LogService.Log(ctx.Args[0], Enums.LogMessageMetaTypes.Debug);
        return new ConsoleCommandResult()
        {
            Success = true,
            Type = Enums.ConsoleMessageTypes.Ok,
            OutMessage = "Logged to DEBUG log tab"
        };
    }

    [ICCommand("CreateSetting", nArgs=2)]
    private static ConsoleCommandResult CreateSetting(ConsoleCommandContext ctx)
    {
        LogService.Log($"Created setting: {ctx.Args[0]}={ctx.Args[1]}", Enums.LogMessageMetaTypes.Debug);
        ConfigService.CreateSetting(ctx.Args[0], ctx.Args[1]);
        return new ConsoleCommandResult()
        {
            Success = true,
            Type = Enums.ConsoleMessageTypes.Ok,
            OutMessage = $"Created setting: {ctx.Args[0]}={ctx.Args[1]}"
        };
    }
}
