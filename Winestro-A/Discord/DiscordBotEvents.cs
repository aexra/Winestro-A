using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Winestro_A.Helpers;
using Winestro_A.Services;

namespace Winestro_A.Discord;

public static partial class DiscordBotService
{
    public static Action<SocketMessage>? ChatOnMessageEventListener { get; set; }
    public static Action? OnReadyEventListener { get; set; }
    public static Action? OnDisconnectedEventListener { get; set; }

    private static Task Ready()
    {
        _interactionService = new(Client.Rest);

        Client.InteractionCreated += async (x) =>
        {
            var ctx = new SocketInteractionContext(_client, x);
            await InteractionService.ExecuteCommandAsync(ctx, null);
        };

        InteractionService.AddModuleAsync<SlashTestModule>(null);

        RunnedAt = TimeHelper.Now;
        OnReadyEventListener?.Invoke();

        return Task.CompletedTask;
    }
    private static Task Disconnected(Exception exception)
    {
        RunnedAt = null;
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
}
