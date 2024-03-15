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
    private static readonly int batchSize = 1920;

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
    private bool SkipRequested = false;

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
    public async void Stop()
    {
        PlayQueue.Clear();
        SkipRequested = true;
        NowPlaying = null;
        await DeleteSelf();
    }
    public void Skip(uint count)
    {
        for (int i = 0;  i < count - 1; i++)
        {
            PlayQueue.Dequeue();
        }
        SkipRequested = true;
    }
    public void Continue()
    {
        if (State == MusicPlayerStates.Paused) State = MusicPlayerStates.Playing;
    }
    public void Pause()
    {
        if (State == MusicPlayerStates.Playing) State = MusicPlayerStates.Paused;
    }
    public void ToggleRepeat()
    {
        IsRepeating = !IsRepeating;
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

            LogService.Log($"Starting playing to [{Guild.Name}] -> [{NowPlaying.Value.Title}]", Enums.LogMessageMetaTypes.Music);

            FFmpegProc = FFmpegHelper.CreateStream(NowPlaying.Value.AudioUrl);

            if (FFmpegProc == null)
            {
                LogService.Error("Cannot create FFmpeg", Enums.LogMessageMetaTypes.Music);
                return;
            }

            using var output = FFmpegProc.StandardOutput.BaseStream;
            using var discord = AudioClient.CreatePCMStream(AudioApplication.Mixed);

            //// TODO: использовать перегрузку CopyToAsync с CancellationToken'ом
            //try { await output.CopyToAsync(discord); }
            //finally { await discord.FlushAsync(); }

            await AudioClient.SetSpeakingAsync(true);

            while (true)
            {
                if (SkipRequested)
                {
                    SkipRequested = false;
                    break;
                }
                if (FFmpegProc.HasExited || discord == null) break;
                if (State == MusicPlayerStates.Paused) continue;

                byte[] buffer = new byte[batchSize];
                var nbytes = await output.ReadAsync(buffer, 0, batchSize);

                if (nbytes <= 0) break;

                try
                {
                    await discord.WriteAsync(buffer, 0, nbytes);
                }
                catch (Exception ex)
                {
                    await discord?.FlushAsync();
                    LogService.Error("Exception in MusicPlayer: " + ex.ToString(), Enums.LogMessageMetaTypes.Music);
                    break;
                }
            }

            await AudioClient.SetSpeakingAsync(false);

            LogService.Log($"Finished playing to [{Guild.Name}] -> [{NowPlaying.Value.Title}]", Enums.LogMessageMetaTypes.Music);

            await discord?.FlushAsync();
        }
    }
    private async Task OnPlayLoopClose()
    {
        await DeleteSelf();
    }

    private async Task DeleteSelf()
    {
        MusicHandler.TryRemoveAudioPlayer(Guild.Id);
        await Channel.DisconnectAsync();
    }
}
