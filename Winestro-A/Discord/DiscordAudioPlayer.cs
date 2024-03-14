using Discord;
using Discord.Audio;
using Microsoft.UI.Xaml.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Winestro_A.Services;
using Winestro_A.Youtube;
using Winestro_A.FFmpeg;

namespace Winestro_A.Discord;

public class DiscordAudioPlayer
{
    public ulong GuildID;
    public IUser user;
    public IAudioClient client;
    public Queue<string> PlayQueue;

    public DiscordAudioPlayer(ulong guildID, IUser user, IAudioClient client)
    {
        GuildID = guildID;
        this.user = user;
        this.client = client;
        PlayQueue = new();
    }

    public async Task Play()
    {
        await RunFFmpeg();
    }
    public async Task ConnectIfNot(IVoiceChannel channel)
    {
        var guild = DiscordBotService.GetGuild(GuildID);
        
        if (guild.AudioClient == null)
        {
            client = await channel.ConnectAsync();
        }
    }

    private async Task RunFFmpeg()
    {
        using var ffmpeg = FFmpegHelper.CreateStream(PlayQueue.Dequeue());

        if (ffmpeg == null)
        {
            LogService.Error("Cannot create FFmpeg");
            return;
        }

        using var output = ffmpeg.StandardOutput.BaseStream;
        using var discord = client.CreatePCMStream(AudioApplication.Mixed);

        LogService.Log("Лог перед ффмпегом", Enums.LogMessageMetaTypes.Debug);

        try { await output.CopyToAsync(discord); }
        finally { await discord.FlushAsync(); }

        LogService.Log("Лог после ффмпега", Enums.LogMessageMetaTypes.Debug);
    }
}
