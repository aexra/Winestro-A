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
using Winestro_A.Structures;

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
        await MusicHandler.KillAllPlayers();
        await _client.LogoutAsync();
    }

    public static void SetupEvents()
    {
        _client.Log += Log;
        _client.Ready += Ready;
        _client.Disconnected += Disconnected;
        _client.MessageReceived += MessageRecieved;
        //_client.UserVoiceStateUpdated += VoiceStateUpdated;
        _client.InteractionCreated += async (x) =>
        {
            LogService.Log($"Interaction created: [{x}]", Enums.LogMeta.Debug);
            var ctx = new SocketInteractionContext(Client, x);
            var res = await InteractionService.ExecuteCommandAsync(ctx, null);

            if (res.IsSuccess)
            {
                LogService.Log($"Interaction executed: [{x}]", Enums.LogMeta.Debug);
            }
            else
            {
                LogService.Error($"Interaction [{x}] execution fail: [{res.Error}]", Enums.LogMeta.Debug);
            }
        };
    }

    public static async Task<ResultManifest> TryRegisterAllCommandsToGuildAsync()
    {
        try
        {
            var commands = await InteractionService.AddModulesToGuildAsync(GetGuild(ulong.Parse((string)ConfigService.Get("DiscordTestGuildID"))), true, InteractionService.Modules.ToArray());
            LogService.Log($"Test commands registered successfully: {commands.Count}");
            return new(true);
        } catch (Exception ex) { return new(false, ex.ToString()); }
    }
    public static async Task<ResultManifest> TryUnregisterAllCommandsFromGuildAsync()
    {
        try
        {
            await InteractionService.RemoveModulesFromGuildAsync(GetGuild(ulong.Parse((string)ConfigService.Get("DiscordTestGuildID"))), InteractionService.Modules.ToArray());
            LogService.Log($"Test commands unregistered successfully");
            return new(true);
        }
        catch (Exception ex) { return new(false, ex.ToString()); }
    }
}
