using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Winestro_A.Services;

namespace Winestro_A.Discord;

public static partial class DiscordBotService
{
    public static Action<SocketMessage>? ChatOnMessageEventListener { get; set; }
    public static Action? OnReadyEventListener { get; set; }
    public static Action? OnDisconnectedEventListener { get; set; }

    private static Task Ready()
    {
        OnReadyEventListener?.Invoke();
        return Task.CompletedTask;
    }
    private static Task Disconnected(Exception exception)
    {
        OnDisconnectedEventListener?.Invoke();
        return Task.CompletedTask;
    }
    private static Task Log(LogMessage msg)
    {
        switch (msg.Severity)
        {
            case LogSeverity.Error:
                LogService.Error(msg.Message);
                break;
            case LogSeverity.Warning:
                LogService.Warning(msg.Message);
                break;
            case LogSeverity.Info:
                LogService.Log(msg.Message);
                break;
            case LogSeverity.Critical:
                LogService.Error(msg.Message);
                break;
            default:
                LogService.Log(msg.Message, Enums.LogMessageMetaTypes.Debug);
                break;
        }
        return Task.CompletedTask;
    }

    private static Task MessageRecieved(SocketMessage msg)
    {
        ChatOnMessageEventListener?.Invoke(msg);
        return Task.CompletedTask;
    }

    private static async Task SlashCommandHandler(SocketSlashCommand command)
    {
        if (!Commands.Keys.Contains(command.Data.Name))
        {
            await command.RespondAsync($"Unknown command **{command.Data.Name}**. Probably this feature was removed but the removement wasn't registered.");
        }
        else
        {
            await Commands[command.Data.Name].Handle(command);
        }
    }
}
