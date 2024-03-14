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
using Discord.WebSocket;

namespace Winestro_A.Discord;

public class DiscordAudioPlayer
{
    public IGuildUser User;
    public IGuild Guild => User.Guild;
    public IAudioClient AudioClient;
    public MusicItem? NowPlaying;
    public Queue<MusicItem> PlayQueue;

    private bool isPlaying = false;
    public bool IsPlaying 
    {
        get => isPlaying;
        set
        {
            if (value == isPlaying) return;
            isPlaying = value;

            if (value == true) Play();
        }
    }
    public bool IsRepeating = false;

    public DiscordAudioPlayer(IGuildUser user, IAudioClient client)
    {
        this.User = user;
        this.AudioClient = client;
        PlayQueue = new();
    }

    public void Enqueue(MusicItem item)
    {
        PlayQueue.Enqueue(item);
    }

    public static DiscordAudioPlayer FromConnectedChannel(IVoiceChannel channel)
    {
        return new DiscordAudioPlayer(((SocketGuild)channel.Guild).CurrentUser, channel.Guild.AudioClient);
    }
    public static async Task<DiscordAudioPlayer> FromUnconnectedChannel(IVoiceChannel channel)
    {
        return new DiscordAudioPlayer(((SocketGuild)channel.Guild).CurrentUser, channel.Guild.AudioClient);
    }
    public static async Task<DiscordAudioPlayer> FromChannel(IVoiceChannel channel)
    {
        return new DiscordAudioPlayer(((SocketGuild)channel.Guild).CurrentUser, channel.Guild.AudioClient ?? await channel.ConnectAsync());
    }


    private void Play()
    {
        Task.Run(PlayLoop);
    }
    private void Stop()
    {
    
    }
    private void Pause()
    {
        
    }
    private async Task PlayLoop()
    {
        while (true)
        {
            if (!IsPlaying) return;
            if (!IsRepeating)
            {
                if (PlayQueue.Count > 0)
                {
                    NowPlaying = PlayQueue.Dequeue();
                }
                else
                {
                    NowPlaying = null;
                }
            }
            if (NowPlaying == null)
            {
                return;
            }

            using var ffmpeg = FFmpegHelper.CreateStream(NowPlaying.Value.AudioUrl);

            if (ffmpeg == null)
            {
                LogService.Error("Cannot create FFmpeg", Enums.LogMessageMetaTypes.Music);
                return;
            }

            using var output = ffmpeg.StandardOutput.BaseStream;
            using var discord = AudioClient.CreatePCMStream(AudioApplication.Mixed);

            // TODO: использовать перегрузку CopyToAsync с CancellationToken'ом
            try { await output.CopyToAsync(discord); }
            finally { await discord.FlushAsync(); }
        }
    }
}
