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
using System.Diagnostics;

namespace Winestro_A.Discord;

public enum MusicPlayerStates
{
    Idle,
    Playing,
    Paused,
    Default
}

public class DiscordAudioPlayer
{
    private static readonly int maxWaitTime = 120;

    public IGuildUser User;
    public IGuild Guild => User.Guild;
    public IAudioClient AudioClient;
    public MusicItem? NowPlaying;
    public Queue<MusicItem> PlayQueue;
    public IVoiceChannel Channel;

    private Process? FFmpegProc = null;
    private MusicPlayerStates _state = MusicPlayerStates.Default;
    public MusicPlayerStates State
    {
        get => _state;
        set
        {
            if (_state == value) return;
            else
            {
                var ls = _state;
                _state = value;
                if (value == MusicPlayerStates.Playing)
                {
                    if (ls != MusicPlayerStates.Idle)
                        Play();
                }
            }
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
        return new DiscordAudioPlayer(((SocketGuild)channel.Guild).CurrentUser, channel.Guild.AudioClient ?? await channel.ConnectAsync()) { Channel = channel };
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
                State = MusicPlayerStates.Idle;
                var counter = 0;
                while (counter <= maxWaitTime)
                {
                    await Task.Delay(1000);
                    counter++;
                    if (State == MusicPlayerStates.Playing)
                    {
                        break;
                    }
                }
                if (counter >= maxWaitTime)
                {
                    await OnPlayLoopClose();
                    return;
                }
                continue;
            }

            FFmpegProc = FFmpegHelper.CreateStream(NowPlaying.Value.AudioUrl);

            if (FFmpegProc == null)
            {
                LogService.Error("Cannot create FFmpeg", Enums.LogMessageMetaTypes.Music);
                return;
            }

            using var output = FFmpegProc.StandardOutput.BaseStream;
            using var discord = AudioClient.CreatePCMStream(AudioApplication.Mixed);

            // TODO: использовать перегрузку CopyToAsync с CancellationToken'ом
            try { await output.CopyToAsync(discord); }
            finally { await discord.FlushAsync(); }
        }
    }
    private async Task OnPlayLoopClose()
    {
        MusicHandler.TryRemoveAudioPlayer(Guild.Id);
        await Channel.DisconnectAsync();
    }
}
