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
    public static IServiceProvider ServiceProvider => _serviceProvider;

    private static DiscordSocketClient _client;
    private static InteractionService _interactionService;
    private static IServiceProvider _serviceProvider;

    public static void Init()
    {
        _serviceProvider = CreateProvider();
        _client = ServiceProvider.GetRequiredService<DiscordSocketClient>();
        _interactionService = ServiceProvider.GetRequiredService<InteractionService>();

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

    private static IServiceProvider CreateProvider()
    {
        var config = new DiscordSocketConfig()
        {
            GatewayIntents = GatewayIntents.All,
            //UseInteractionSnowflakeDate = false
        };

        var servConfig = new InteractionServiceConfig() 
        { 
            AutoServiceScopes = true 
        };

        var collection = new ServiceCollection()
            .AddSingleton(config)
            .AddSingleton<DiscordSocketClient>()
            .AddSingleton(servConfig)
            .AddSingleton<InteractionService>();

        return collection.BuildServiceProvider();
    }

    public static void SetupEvents()
    {
        _client.Log += Log;
        _client.Ready += Ready;
        _client.Disconnected += Disconnected;
        _client.MessageReceived += MessageRecieved;
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
