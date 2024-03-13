using Discord;
using Discord.Audio;
using Discord.Interactions;
using Winestro_A.Services;
using Winestro_A.Youtube;


namespace Winestro_A.Discord;

public class SlashTestModule : InteractionModuleBase<SocketInteractionContext>
{
    public override void OnModuleBuilding(InteractionService commandService, ModuleInfo module)
    {
        base.OnModuleBuilding(commandService, module);

        LogService.Log("Slash test commands module loaded successfully");
    }

    public override Task BeforeExecuteAsync(ICommandInfo command)
    {
        LogService.Log($"Command [{command.Name}] is going to be invoked...", Enums.LogMessageMetaTypes.Debug);
        return base.BeforeExecuteAsync(command);
    }

    [SlashCommand("test", "Команда для теста")]
    public async Task Test()
    {
        LogService.Log("Test discord bot command invoked", Enums.LogMessageMetaTypes.Debug);
        await RespondAsync("✅ Ы");
    }

    [SlashCommand("testdeffered", "Команда для теста отложенных команд")]
    public async Task DeferredTest()
    {
        LogService.Log("Deferred test discord bot command invoked", Enums.LogMessageMetaTypes.Debug);
        await DeferAsync();
        await ModifyOriginalResponseAsync(props => { props.Content = "✅ Deferred Ы"; });
    }

    [SlashCommand("join", "Захожу в голосовой канал", runMode: RunMode.Async)]
    public async Task JoinChannel(IVoiceChannel channel = null)
    {
        // Get the audio channel
        channel = channel ?? (Context.User as IGuildUser)?.VoiceChannel;
        if (channel == null) { await RespondAsync("📛 Ты или сам зайди в канал, или скажи в какой мне зайти 👺"); return; }

        // For the next step with transmitting audio, you would want to pass this Audio Client in to a service.
        var audioClient = await channel.ConnectAsync();

        await RespondAsync($"✅ Зашел в {channel.Mention}");
    }

    [SlashCommand("leave", "Ухожу из голосового канала", runMode: RunMode.Async)]
    public async Task LeaveChannel()
    {
        if (Context.Guild.CurrentUser.VoiceChannel != null)
        {
            var channel = Context.Guild.CurrentUser.VoiceChannel;
            await channel.DisconnectAsync();
            await RespondAsync($"✅ Вышел из {channel.Mention}");
        }
        else
        {
            await RespondAsync($"📛 Чтобы выйти откуда-то, надо быть где-то ☝️");
        }
    }

    [SlashCommand("playtestfile", "Играет аудиофайл, сохраненный в приложении", runMode:RunMode.Async)]
    public async Task PlayTest()
    {
        // Get the audio channel
        var channel = (Context.User as IGuildUser)?.VoiceChannel;
        if (channel == null) { await RespondAsync("📛 Ты или сам зайди в канал, или скажи в какой мне зайти 👺"); return; }

        // For the next step with transmitting audio, you would want to pass this Audio Client in to a service.
        var audioClient = await channel.ConnectAsync();

        await RespondAsync($"✅ Зашел в {channel.Mention}");

        using var ffmpeg = FFmpeg.FFmpeg.CreateStream((await Extractor.GetAudioStreamHighestQuality("https://www.youtube.com/watch?v=jKikelM3FWM")).Url);
        using var output = ffmpeg.StandardOutput.BaseStream;
        using var discord = audioClient.CreatePCMStream(AudioApplication.Mixed);
        try { await output.CopyToAsync(discord); }
        finally { await discord.FlushAsync(); }
    }
}
