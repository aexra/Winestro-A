using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Winestro_A.Helpers;
using Winestro_A.Services;

namespace Winestro_A.Discord;

public static partial class DiscordBotService
{
    public static Action<SocketMessage>? ChatOnMessageEventListener { get; set; }
    public static Action? OnReadyEventListener { get; set; }
    public static Action? OnDisconnectedEventListener { get; set; }

    private static async Task Ready()
    {
        await InteractionService.AddModulesAsync(Assembly.GetEntryAssembly(), ServiceProvider);

        Client.InteractionCreated += async (x) =>
        {
            var ctx = new SocketInteractionContext(Client, x);
            var res = await InteractionService.ExecuteCommandAsync(ctx, ServiceProvider);
        };

        RunnedAt = TimeHelper.Now;
        OnReadyEventListener?.Invoke();
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
