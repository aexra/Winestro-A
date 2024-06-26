﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Winestro_A.Discord;

public static class MusicHandler
{
    public static Dictionary<ulong, MusicPlayer> PlayersDict = new();

    public static async Task KillAllPlayers()
    {
        foreach (var player in PlayersDict.Values)
        {
            await player.Kill();
        }
    }

    public static bool TryGetPlayer(ulong guild, out MusicPlayer? player)
    {
        return PlayersDict.TryGetValue(guild, out player);
    }
    public static bool TryRemoveAudioPlayer(ulong guild)
    {
        try
        {
            PlayersDict.Remove(guild);
            return true;
        }
        catch { return false; }
    }
    public static bool TryAddAudioPlayer(MusicPlayer player)
    {
        return PlayersDict.TryAdd(player.Guild.Id, player);
    }
}
