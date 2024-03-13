using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Winestro_A.Discord;

public static partial class DiscordBotService
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
}
