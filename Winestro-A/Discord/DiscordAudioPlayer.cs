using Discord;
using Discord.Audio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

    public async Task ConnectIfNot(IVoiceChannel channel)
    {
        var guild = DiscordBotService.GetGuild(GuildID);
        
        if (guild.AudioClient == null)
        {
            client = await channel.ConnectAsync();
        }
    }
}
