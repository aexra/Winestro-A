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
    public static async Task<List<IGuildChannel>> GetGuildChannelsAsync(IGuild guild) => (await guild.GetChannelsAsync()).ToList();
    public static async Task<List<IGuildChannel>> GetGuildChannelsAsync(ulong guild) => (await ((IGuild)_client.GetGuild(guild)).GetChannelsAsync()).ToList();
    public static async Task<List<ITextChannel>> GetGuildTextChannelsAsync(IGuild guild) => (await guild.GetTextChannelsAsync()).ToList();
    public static async Task<List<ITextChannel>> GetGuildTextChannelsAsync(ulong guild) => (await ((IGuild)_client.GetGuild(guild)).GetTextChannelsAsync()).Where(x => x.GetChannelType() == ChannelType.Text).ToList();
}
