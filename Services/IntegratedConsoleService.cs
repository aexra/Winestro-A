using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Controls;
using Windows.ApplicationModel.UserDataTasks.DataProvider;
using Winestro_A.Structures;

namespace Winestro_A.Services;
public class IntegratedConsoleService
{
    public static ObservableCollection<TextBlock> ConsoleHistory { get; private set; } = new();
    private static readonly Dictionary<string, ConsoleCommandTemplate> CommandsMap = new()
    {
        { 
            "test", new ConsoleCommandTemplate() { 
                Name="test", 
                nArgs=0, 
                KwargsKeys=null, 
                Function=Test 
            } 
        },
    };

    public static bool TryRun(string promt, out CommandResult result)
    {
        ConsoleHistory.Add(new TextBlock() { Text="$ " + promt });

        // Заранее зададим пустой результат с необработанным исключением
        result = new CommandResult() { Success = false, OutMessage = "Unhandled exception" };
        
        // Пробуем спарсить команду
        var parseResult = TryParse(promt, out var cmd, out var error);

        // Не спарсили?
        if (!parseResult)
        {
            result.OutMessage = error;
            result.Success = false;
        }
        else
        {
            var command = cmd.Value;

            // Спарсили.
            if (CommandsMap.Keys.ToList().Contains(command.Name))
            {
                var template = CommandsMap[command.Name];
                if (template.nArgs != command.Args.Count())
                {
                    result.OutMessage = $"Arguments error in command [{command.Name}]: expected {template.nArgs}, {command.Args.Count()} were given";
                    result.Success = false;
                }
                else
                {
                    var goodKwargsKeys = true;

                    foreach (var key in command.Kwargs.Keys)
                    {
                        template.KwargsKeys ??= Array.Empty<string>();
                        if (!template.KwargsKeys.Contains(key))
                        {
                            result.OutMessage = $"Kwarg with key [{key}] not found for command [{command.Name}]";
                            result.Success = false;
                            goodKwargsKeys = false;
                        }
                    }

                    if (goodKwargsKeys)
                    {
                        result = template.Function(command.Args, command.Kwargs);
                    }
                }
            }
            else
            {
                result.OutMessage = $"[{cmd.Value.Name}] command not found";
                result.Success = false;
            }
        }

        ConsoleHistory.Add(new TextBlock() { Text = "> " + result.OutMessage, TextWrapping=Microsoft.UI.Xaml.TextWrapping.Wrap });
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
        String[] parts = promt.Split(' ');

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



    private static CommandResult Test(List<String> args, Dictionary<string, string> kwargs)
    {
        return new CommandResult() {
            Success = true,
            OutMessage = $"Hello, world!"
        };
    }
}
