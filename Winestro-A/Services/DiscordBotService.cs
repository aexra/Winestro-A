using Discord;
using Discord.WebSocket;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Winestro_A.Services;

public class DiscordBotService
{
    private static DiscordSocketClient _client;
    public static Discord.ConnectionState ConnectionState => _client.ConnectionState;

    public static void Init()
    {
        var config = new DiscordSocketConfig()
        {
            GatewayIntents = GatewayIntents.All
        };
        _client = new DiscordSocketClient(config);
        _client.Log += Log;
        _client.Ready += Ready;
        _client.Disconnected += Disconnected;
    }

    public static async Task Toggle()
    {
        if (ConnectionState == Discord.ConnectionState.Connected)
        {
            await Stop();
        }
        else if (ConnectionState == Discord.ConnectionState.Disconnected)
        {
            await Run();
        }
    }

    public static async Task Run()
    {
        LogService.Log("Launching bot...");
        var token = ConfigService.Get("DiscordApiKey") ?? "No Token";
        await _client.LoginAsync(TokenType.Bot, (string)token);
        await _client.StartAsync();
    }

    public static async Task Stop()
    {
        LogService.Log("Stopping bot...");
        await _client.LogoutAsync();
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

    private static Task Ready()
    {
        LogService.Log("Discord Bot connected successfully");
        return Task.CompletedTask;
    }
    private static Task Disconnected(Exception exception)
    {
        LogService.Log("Discord Bot disconnected successfully");
        return Task.CompletedTask;
    }
}
