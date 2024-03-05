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
    private static Dictionary<string, ConsoleCommandTemplate> CommandsMap = new();

    public static bool TryRun(string promt, out CommandResult result)
    {
        // Заранее зададим сообщение ошибки null
        result = new CommandResult() { Success = false, OutMessage = "Unhandled exception" };
        
        // Пробуем спарсить команду
        var parseResult = TryParse(promt, out var cmd, out var error);

        // Не спарсили?
        if (!parseResult)
        {
            result.OutMessage = error;
            return false;
        }

        var command = cmd.Value;
        
        // Спарсили.
        if (CommandsMap.Keys.ToList().Contains(command.Name))
        {
            var template = CommandsMap[command.Name];
            if (template.nArgs != command.Args.Count() || template.KwargsKeys.Count() != command.Kwargs.Keys.Count())
            {
                result.OutMessage = $"Неверное количество аргументов команды <{command.Name}>: ожидалось {template.nArgs}/{template.KwargsKeys.Count()}, получено {command.Args.Count()}/{command.Kwargs.Keys.Count()}";
                return false;
            }
            else
            {
                result = template.Function(command.Args, command.Kwargs);
            }
        }
        else
        {
            result.OutMessage = $"<{cmd.Value.Name}> не является внутренней командой";
            return false;
        }


        return true;
    }

    private static bool TryParse(string promt, out ConsoleCommand? cmd, out string? errorMesage)
    {
        // Заранее присвоим cmd и errorMessage null 
        cmd = null;
        errorMesage = null;

        // Если строка пустая или состоит только из пробелов, то вернем 0
        if (String.IsNullOrWhiteSpace(promt))
        {
            errorMesage = "Команда пустая или состоит из пустых символов.\nТы как это сделал?..";
            return false;
        }

        // Разобьем всю строку на куски
        String[] parts = promt.Split(' ');

        // Проверим является ли первое слово названием команды (только латинские буквы)
        if (!IsWord(parts[0]))
        {
            errorMesage = $"<{parts[0]}> не является именем команды";
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
        var result = Regex.IsMatch(promt, "^[a-zA-Z]+={1}.+$");
        
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
            OutMessage = "This is test command"
        };
    }
}
