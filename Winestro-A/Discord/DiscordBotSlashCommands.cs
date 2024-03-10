using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Winestro_A.Services;

namespace Winestro_A.Discord;

public partial class DiscordBotService
{
    private static Dictionary<string, SlashCommand> Commands = new()
    {
        { "test", new("test", "This is test command for debug", HandleTest) }
    };

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

    private static async Task HandleTest(SocketSlashCommand command)
    {
        await command.RespondAsync("Hello, world!");
    }

    public static async Task DeleteSlashCommands()
    {
        await _client.Rest.DeleteAllGlobalCommandsAsync();
    }
}
