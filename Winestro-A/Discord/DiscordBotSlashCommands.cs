using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Winestro_A.Attributes;
using Winestro_A.Services;
using Winestro_A.Structures;

namespace Winestro_A.Discord;

public static partial class DiscordBotService
{
    private static Dictionary<string, SlashCommand> Commands = new();

    public static void InitSlashCommands()
    {
        var methods = typeof(DiscordBotService).GetMethods(BindingFlags.Static | BindingFlags.NonPublic);
        var counter = 0;

        foreach (var method in methods)
        {
            var found = false;
            SlashCommandAttribute? scattr = null;

            foreach (var attrib in method.GetCustomAttributes())
            {
                if (attrib is SlashCommandAttribute _scattr)
                {
                    found = true;
                    scattr = _scattr;
                    break;
                }
            }

            if (found)
            {
                Commands.Add(scattr.Name, new(
                    scattr.Name,
                    scattr.Description,
                    method.CreateDelegate<Func<SocketSlashCommand, Task>>(),
                    scattr.IsGlobal
                ));
                LogService.Log($"Slash command [{scattr.Name}] compiled", Enums.LogMessageMetaTypes.Debug);
                counter++;
            }
        }

        LogService.Log($"All slash commands successfully compiled: {counter}");
    }

    public static async Task<bool> RegisterSlashCommands()
    {


        LogService.Log("Discord Bot slash commands have been registered");
        return true;
    }
    public static async Task<bool> RegisterTestSlashCommands()
    {
        // TRY REGISTER ALL COMMANDS FROM COMMANDS DICT
        try
        {
            foreach (var command in Commands.Values)
            {
                await command.Register(_client);
            }
        }
        catch (Exception ex)
        {
            LogService.Error("Error registering Discord Bot *TEST* slash commands.\n" + ex.ToString());
            return false; 
        }

        // LOG IF SUCCESSFUL
        LogService.Log("Discord Bot *TEST* slash commands have been registered");
        return true;
    }
    public static async Task DeleteSlashCommands()
    {
        await _client.Rest.DeleteAllGlobalCommandsAsync();
    }

    [SlashCommand("test", "This is test command for debug")]
    private static async Task HandleTest(SocketSlashCommand command)
    {
        await command.RespondAsync("Hello, world!");
    }
}
