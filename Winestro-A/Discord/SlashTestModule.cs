using System.Diagnostics;
using Discord;
using Discord.Audio;
using Discord.Interactions;
using Winestro_A.Services;
using Winestro_A.Structures;
using Winestro_A.Youtube;


namespace Winestro_A.Discord;

public class SlashTestModule : InteractionModuleBase<SocketInteractionContext>
{
    private static Dictionary<ulong, DiscordAudioPlayer> PlayersDict => MusicHandler.PlayersDict;

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

        using var ffmpeg = FFmpeg.FFmpegHelper.CreateStream((await Extractor.GetAudioStreamHighestQuality("https://www.youtube.com/watch?v=jKikelM3FWM")).Url);
        using var output = ffmpeg.StandardOutput.BaseStream;
        using var discord = audioClient.CreatePCMStream(AudioApplication.Mixed);
        try { await output.CopyToAsync(discord); }
        finally { await discord.FlushAsync(); }
    }

    [SlashCommand("play", "Продолжает воспроизведение музыки или добавляет новую в очередь")]
    public async Task Play(string promt = "")
    {
        await DeferAsync();

        // TODO: yt search before using promt (promt to url)
        if (promt == "") await Continue();
        else await Enqueue(promt);
    }
    private async Task Continue()
    {
        var guild = Context.Guild;
        if (MusicHandler.PlayersDict.ContainsKey(guild.Id))
        {
            var player = PlayersDict[guild.Id];
            if (player.PlayQueue.Count() > 1 || player.NowPlaying != null)
            {
                MusicHandler.PlayersDict[guild.Id].State = MusicPlayerStates.Playing;
                await ModifyOriginalResponseAsync(p => p.Content = ":notes: Врубил твое музло");
            }
            else
            {
                await ModifyOriginalResponseAsync(p => p.Content = "⚠️ Мне нечего врубать, добавь чё то в очередь");
            }
        }
        else
        {
            await ModifyOriginalResponseAsync(p => p.Content = "📛 Очередь пустая, добавь музла");
        }
    }
    private async Task Enqueue(string promt)
    {
        DiscordAudioPlayer? player;
        IVoiceChannel? channel;
        MusicItem? item;

        item = await Extractor.GetMusicItemAsync(promt);
        if (item == null)
        {
            await RespondAsync("📛 Не смог найти ничего по твоему запросу");
            return;
        }
        
        MusicHandler.TryGetPlayer(Context.Guild.Id, out player);
        channel = (Context.User as IGuildUser)?.VoiceChannel;

        if (player == null && channel == null)
        {
            await ModifyOriginalResponseAsync(p => p.Content = "📛 Зайди в канал чтобы создать новый плейлист");
            return;
        }

        if (player == null && channel != null)
        {
            player = await DiscordAudioPlayer.FromChannel(channel);
            MusicHandler.TryAddAudioPlayer(player);
        }

        player.Enqueue(item.Value);
        player.State = MusicPlayerStates.Playing;

        await ModifyOriginalResponseAsync(p => p.Content = $":notes: Добавил твое музло в очередь: **{item.Value.Title}**");
    }

    [SlashCommand("queue", "Выводит очередь воспроизведения музыки")]
    public async Task Queue(uint max = 5)
    {
        await DeferAsync();

        if (MusicHandler.TryGetPlayer(Context.Guild.Id, out var player))
        {
            var now = player.NowPlaying;

            if (now == null)
            {

                await ModifyOriginalResponseAsync(p => p.Content = "📛 Очереди нет, че тебе показывать?");
                return;
            }

            var embed = new EmbedBuilder
            {
                Title = $"Сейчас играет: {now.Value.Title}",
                Description = player.PlayQueue.Count == 0 ? "" : $"Первые {MathF.Min(max, player.PlayQueue.Count)} из {player.PlayQueue.Count} в очереди:",
                Color = Color.Magenta
            };
            for (var i = 0; i < player.PlayQueue.Count; i++)
            {
                var video = player.PlayQueue.ElementAt(i);
                embed.AddField($"{i+1}. {video.Title}", $"{video.Url}");
            }
            await ModifyOriginalResponseAsync(p => p.Embed = embed.Build());
        }
        else
        {
            await ModifyOriginalResponseAsync(p => p.Content = "📛 Очереди нет, че тебе показывать?");
        }
    }
}
