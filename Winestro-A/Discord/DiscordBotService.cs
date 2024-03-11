using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Winestro_A.Services;

namespace Winestro_A.Discord;

public static partial class DiscordBotService
{
    private static DiscordSocketClient _client;
    private static InteractionService _interactionService;

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
        _client.MessageReceived += MessageRecieved;
        _client.SlashCommandExecuted += SlashCommandHandler;

        _interactionService = new(_client.Rest);

        InitSlashCommands();
    }
    public static async Task Toggle()
    {
        if (ConnectionState == ConnectionState.Connected)
        {
            await Stop();
        }
        else if (ConnectionState == ConnectionState.Disconnected)
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
