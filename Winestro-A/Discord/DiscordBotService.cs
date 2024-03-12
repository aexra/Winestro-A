using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Winestro_A.Services;

namespace Winestro_A.Discord;

public static partial class DiscordBotService
{
    public static DiscordSocketClient Client => _client;
    public static InteractionService InteractionService => _interactionService;

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

        _interactionService = new(_client.Rest);
        //_interactionService.AddModuleAsync<DiscordSlashCommandsModule>(null);
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
