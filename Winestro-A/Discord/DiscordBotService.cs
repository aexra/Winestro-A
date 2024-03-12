using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;
using Windows.Devices.Geolocation;
using Winestro_A.Services;

namespace Winestro_A.Discord;

public static partial class DiscordBotService
{
    public static DiscordSocketClient Client => _client;
    public static InteractionService InteractionService => _interactionService;

    private static readonly DiscordSocketConfig socketConf = new()
    {
        GatewayIntents = GatewayIntents.All,
    };
    private static readonly InteractionServiceConfig intsConf = new()
    {
        AutoServiceScopes = true
    };

    private static DiscordSocketClient _client;
    private static InteractionService _interactionService;

    public static void Init()
    {
        _client = new(socketConf);
        _interactionService = new(_client, intsConf);

        SetupEvents();
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

    public static void SetupEvents()
    {
        _client.Log += Log;
        _client.Ready += Ready;
        _client.Disconnected += Disconnected;
        _client.MessageReceived += MessageRecieved;
        _client.InteractionCreated += async (x) =>
        {
            var ctx = new SocketInteractionContext(Client, x);
            var res = await InteractionService.ExecuteCommandAsync(ctx, null);
        };
    }

    public static async Task<bool> RegisterTestCommandsAsync()
    {
        try
        {
            var commands = await InteractionService.RegisterCommandsToGuildAsync(ulong.Parse((string)ConfigService.Get("DiscordTestGuildID")), true);
            LogService.Log($"Test command registered successfully: {commands.Count}");
            return true;
        } catch (Exception ex) { return false; }
    }
}
