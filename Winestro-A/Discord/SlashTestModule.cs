using Discord;
using Discord.Interactions;
using Winestro_A.Services;


namespace Winestro_A.Discord;

public class SlashTestModule : InteractionModuleBase<SocketInteractionContext>
{
    public override void OnModuleBuilding(InteractionService commandService, ModuleInfo module)
    {
        base.OnModuleBuilding(commandService, module);

        LogService.Log("Slash test commands module loaded successfully");
    }

    [SlashCommand("test", "Команда для теста")]
    public async Task Test()
    {
        await RespondAsync("✅ Ы");
    }

    [SlashCommand("join", "Заходит в голосовой канал", runMode: RunMode.Async)]
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
        var vc = Context.Guild.CurrentUser.VoiceChannel;
        if (vc != null)
        {
            // Retrieve audioclient from static (guild: ac)
        }
        else
        {
            await RespondAsync($"📛 Я не в канале!");
        }
    }
}
