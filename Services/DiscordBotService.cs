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
    public static Discord.ConnectionState ConnectionState { get => _client.ConnectionState; private set => ConnectionState = value; }

    public static void Init()
    {
        _client = new DiscordSocketClient();
        _client.Log += Log;
    }

    public static async Task Run()
    {
        var token = ConfigService.Get("DiscordApiKey") ?? "No Token";
        await _client.LoginAsync(TokenType.Bot, (string)token);
        await _client.StartAsync();
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
}
