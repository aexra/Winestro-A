using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Winestro_A.Discord;

public static class MusicHandler
{
    public static Dictionary<ulong, DiscordAudioPlayer> PlayersDict = new();

    public static bool TryGetPlayer(ulong guild, out DiscordAudioPlayer? player)
    {
        return PlayersDict.TryGetValue(guild, out player);
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
