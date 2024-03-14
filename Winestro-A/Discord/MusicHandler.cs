using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Winestro_A.Discord;

public static class MusicHandler
{
    public static Dictionary<ulong, DiscordAudioPlayer> PlayersDict = new Dictionary<ulong, DiscordAudioPlayer>();

    public static DiscordAudioPlayer? GetAudioPlayer(ulong guild)
    {
        var result = PlayersDict.TryGetValue(guild, out var player);
        if (result)
        {
            return player;
        }
        else
        {
            return null;
        }
    }
    public static void RemoveAudioPlayer(ulong guild)
    {
        try
        {
            PlayersDict.Remove(guild);
        }
        catch { }
    }
    public static void AddAudioPlayer(DiscordAudioPlayer player)
    {
        PlayersDict.TryAdd(player.Guild.Id, player);
    }
}
