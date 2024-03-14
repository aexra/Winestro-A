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
using Winestro_A.Structures;

namespace Winestro_A.Discord;

public class DiscordAudioPlayer
{
    public IGuildUser User;
    public IGuild Guild => User.Guild;
    public IAudioClient AudioClient;
    public MusicItem? NowPlaying;
    public Queue<MusicItem> PlayQueue;

    // TODO: setter for continue/pause
    public bool IsPlaying = false;

    public DiscordAudioPlayer(IGuildUser user, IAudioClient client)
    {
        this.User = user;
        this.AudioClient = client;
        PlayQueue = new();
    }



    //public async Task Play()
    //{
    //    await RunFFmpeg();
    //}
    //public async Task ConnectIfNot(IVoiceChannel channel)
    //{   
    //    if (Guild.AudioClient == null)
    //    {
    //        AudioClient = await channel.ConnectAsync();
    //    }
    //}

    //private async Task RunFFmpeg()
    //{
    //    using var ffmpeg = FFmpegHelper.CreateStream(PlayQueue.Dequeue());

    //    if (ffmpeg == null)
    //    {
    //        LogService.Error("Cannot create FFmpeg");
    //        return;
    //    }

    //    using var output = ffmpeg.StandardOutput.BaseStream;
    //    using var discord = AudioClient.CreatePCMStream(AudioApplication.Mixed);

    //    LogService.Log("Лог перед ффмпегом", Enums.LogMessageMetaTypes.Debug);

    //    try { await output.CopyToAsync(discord); }
    //    finally { await discord.FlushAsync(); }

    //    LogService.Log("Лог после ффмпега", Enums.LogMessageMetaTypes.Debug);
    //}
}
