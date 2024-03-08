using Discord;
using Discord.WebSocket;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Winestro_A.Discord;

public partial class DiscordBotService
{
    private static DiscordSocketClient _client;

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
        await _client.LogoutAsync();
    }
}
