using Discord;
using Discord.Interactions;
using Winestro_A.Services;
using Winestro_A.Structures;
using Winestro_A.Youtube;


namespace Winestro_A.Discord;

public class MusicClashModule : InteractionModuleBase<SocketInteractionContext>
{
    private static Dictionary<ulong, MusicPlayer> PlayersDict => MusicHandler.PlayersDict;

    public override void OnModuleBuilding(InteractionService commandService, ModuleInfo module)
    {
        base.OnModuleBuilding(commandService, module);

        LogService.Log("Music slash commands module loaded successfully");
    }

    [SlashCommand("play", "Продолжает воспроизведение музыки или добавляет новую в очередь", runMode: RunMode.Async)]
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
                MusicHandler.PlayersDict[guild.Id].Continue();
                await ModifyOriginalResponseAsync(p => p.Content = ":arrow_forward: Врубил твое музло");
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
        MusicPlayer? player;
        IVoiceChannel? channel;
        MusicItem? item;

        if (!promt.Contains("https://"))
        {
            var video = await Extractor.Search(promt);
            if (video == null)
            {
                await RespondAsync("📛 Не смог найти ничего по твоему запросу");
                return;
            }
            promt = video.Id;
        }

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
            player = await MusicPlayer.FromChannel(channel);
            MusicHandler.TryAddAudioPlayer(player);
        }

        player.Enqueue(item.Value);
        player.State = MusicPlayerStates.Playing;

        await ModifyOriginalResponseAsync(p => p.Content = $":notes: Добавил твое музло в очередь: **{item.Value.Title} - {item.Value.Duration}**");
    }

    [SlashCommand("skip", "Пропускает текущий трек")]
    public async Task Skip(uint count = 1)
    {
        if (MusicHandler.TryGetPlayer(Context.Guild.Id, out var player))
        {
            player?.Skip(count);
            await RespondAsync($"⏬ Пропустил тебе {count} музлa");
        }
        else
        {
            await RespondAsync("⚠️ Чо пропускать если нечево..");
        }
    }

    [SlashCommand("pause", "Приостанавливает воспроизведение музла")]
    public async Task Pause()
    {
        if (MusicHandler.TryGetPlayer(Context.Guild.Id, out var player))
        {
            player?.Pause();
            await RespondAsync(":pause_button: Приостановил воспроизведение музла");
        }
        else
        {
            await RespondAsync("⚠️ Чо останавливать если нечево..");
        }
    }

    [SlashCommand("repeat", "Залупливает воспроизведение музла")]
    public async Task Repeat()
    {
        if (MusicHandler.TryGetPlayer(Context.Guild.Id, out var player))
        {
            player?.ToggleRepeat();
            await RespondAsync(player.IsRepeating ? $"🔂 Залупил тебе музло" : $"🔁 Разлупил тебе музло");
        }
        else
        {
            await RespondAsync("⚠️ Чо залупливать если нечево..");
        }
    }

    [SlashCommand("clear", "Очищает очередь музла (текущее останется)")]
    public async Task ClearQueue()
    {
        if (MusicHandler.TryGetPlayer(Context.Guild.Id, out var player))
        {
            player?.ClearQueue();
            await RespondAsync(":white_check_mark: Очистил тебе очередь");
        }
        else
        {
            await RespondAsync("⚠️ Чо очищать если очереди нету..");
        }
    }

    [SlashCommand("stop", "Останавливает воспроизведение музла и очищает очередь")]
    public async Task Stop()
    {
        if (MusicHandler.TryGetPlayer(Context.Guild.Id, out var player))
        {
            player?.Pause();
            await RespondAsync("⏹️ Кончил воспроизведение музла");
        }
        else
        {
            await RespondAsync("⚠️ Чо останавливать если нечево..");
        }
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

            TimeSpan TotalDuration = TimeSpan.Zero;

            foreach (var item in player.PlayQueue)
            {
                TotalDuration += item.Duration ?? TimeSpan.Zero;
            }

            var embed = new EmbedBuilder
            {
                Title = $"Сейчас играет: {now.Value.Title} - {now.Value.Duration}",
                Description = player.PlayQueue.Count == 0 ? "" : $"Первые {MathF.Min(max, player.PlayQueue.Count)} из {player.PlayQueue.Count} в очереди - {TotalDuration}",
                Color = Color.Magenta
            };
            for (var i = 0; i < player.PlayQueue.Count; i++)
            {
                var video = player.PlayQueue.ElementAt(i);
                embed.AddField($"{i + 1}. {video.Title} - {video.Duration}", $"{video.Url}");
            }
            await ModifyOriginalResponseAsync(p => p.Embed = embed.Build());
        }
        else
        {
            await ModifyOriginalResponseAsync(p => p.Content = "📛 Очереди нет, че тебе показывать?");
        }
    }
}
