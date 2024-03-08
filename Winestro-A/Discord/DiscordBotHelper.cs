using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;

namespace Winestro_A.Discord;

public partial class DiscordBotService
{
    public static ConnectionState ConnectionState => _client.ConnectionState;
    public static List<IGuild> Guilds
    {
        get
        {
            if (ConnectionState != ConnectionState.Connected) return new List<IGuild> { };
            List<IGuild> guilds = new();
            foreach (var guild in _client.Guilds)
            {
                guilds.Add(guild);
            }
            return guilds;
        }
    }

    public static List<IGuild> GetGuilds() => Guilds;
    public static async Task<List<IGuildChannel>> GetGuildChannels(IGuild guild) => (await guild.GetChannelsAsync()).ToList();
    public static async Task<List<IGuildChannel>> GetGuildChannels(ulong guild) => (await ((IGuild)_client.GetGuild(guild)).GetChannelsAsync()).ToList();
    public static async Task<List<ITextChannel>> GetGuildTextChannels(IGuild guild) => (List<ITextChannel>)(await guild.GetChannelsAsync()).ToList().Where(x => x is ITextChannel);
    public static async Task<List<ITextChannel>> GetGuildTextChannels(ulong guild) => (List<ITextChannel>)(await ((IGuild)_client.GetGuild(guild)).GetChannelsAsync()).ToList().Where(x => x is ITextChannel);
}
