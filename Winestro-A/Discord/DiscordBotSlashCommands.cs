using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Winestro_A.Attributes;
using Winestro_A.Services;
using Winestro_A.Structures;
using Winestro_A.Youtube;
using Ints = Discord.Interactions;

namespace Winestro_A.Discord;

public static partial class DiscordBotService
{
    private static Dictionary<string, SlashCommand> Commands = new();
    private static Dictionary<string, Func<SlashCommandBuilder, SlashCommandBuilder>> CustomPropertiesBuilders = new()
    {
        { "gasu", (SlashCommandBuilder builder) => builder.AddOption("url", ApplicationCommandOptionType.String, "URL to video to search", true) },
    };

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
                SlashCommand command = new(
                    scattr.Name,
                    scattr.Description,
                    method.CreateDelegate<Func<SocketSlashCommand, Task>>(),
                    scattr.IsGlobal
                );
                if (CustomPropertiesBuilders.TryGetValue(command.Name, out var builder))
                {
                    command.PropertiesBuilder = builder;
                }
                Commands.Add(scattr.Name, command);
                LogService.Log($"Slash command [{scattr.Name}] compiled", Enums.LogMessageMetaTypes.Debug);
                counter++;
            }
        }

        LogService.Log($"All slash commands successfully compiled: {counter}");
    }

    public static async Task<bool> RegisterGlobalSlashCommands()
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
    private static async Task Test(SocketSlashCommand command)
    {
        await command.RespondAsync("Hello, world!");
    }

    [SlashCommand("gasu", "Returns audio stream url")]
    private static async Task GetAudioStreamUrl(SocketSlashCommand command)
    {
        await command.DeferAsync();
        var info = await Extractor.GetAudioStreamHighestQuality((string)command.Data.Options.First().Value);
        if (info == null)
        {
            await command.ModifyOriginalResponseAsync((p) => p.Content = $"📛 Cannot find video by URL **{(string)command.Data.Options.First().Value}**");
        }
        else
        {
            await command.ModifyOriginalResponseAsync((p) => p.Content = "✅ Your URL: " + info.Url);
        }
    }

    [SlashCommand("join", "joins a voice channel")]
    [Ints.SlashCommand("join", "joins a voice channel", runMode:Ints.RunMode.Async)]
    private static async Task JoinChannel(SocketSlashCommand command)
    {
        var channel = (command.User as IGuildUser)?.VoiceChannel;
        if (channel == null)
        {
            await command.RespondAsync("⚠️ User **must** be in a voice channel to use this command");
            return;
        }

        var audioClient = await channel.ConnectAsync();
    }
}
