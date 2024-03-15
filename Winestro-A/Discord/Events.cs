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
        try
        {
            await InteractionService.AddModulesAsync(Assembly.GetEntryAssembly(), null);
        }
        catch (Exception ex)
        {
            LogService.Error("Discord Ready event has thrown an exception: " + ex);
        }

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
                LogService.Log(msg.Message, Enums.LogMeta.Debug);
                break;
        }
        return Task.CompletedTask;
    }

    private static Task MessageRecieved(SocketMessage msg)
    {
        ChatOnMessageEventListener?.Invoke(msg);
        return Task.CompletedTask;
    }

    // Это ивент который срабатывает после ЛЮБОГО изменения состояния войса
    // TODO: проверять если это (SocketGuildUser)user == client.RestUser чтобы чекать состояние бота
    private static Task VoiceStateUpdated(SocketUser user, SocketVoiceState s1, SocketVoiceState s2)
    {
        LogService.Log($"s1: {s1}, s2: {s2}");
        return Task.CompletedTask;
    }
}
