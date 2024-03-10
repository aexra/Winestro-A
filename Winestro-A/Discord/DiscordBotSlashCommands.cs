using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Winestro_A.Services;

namespace Winestro_A.Discord;

public partial class DiscordBotService
{
    public static async Task RegisterSlashCommands()
    {


        LogService.Log("Discord Bot slash commands have been registered");
    }
    public static async Task RegisterTestSlashCommands()
    {
        IGuild guild;
        try
        {
            guild = GetGuild(ulong.Parse(ConfigService.Get("DiscordTestGuildID").ToString()));
        }
        catch { return; }

        var guildCommand = new SlashCommandBuilder();
        guildCommand.WithName("winestro-test");
        guildCommand.WithDescription("This is my first guild slash command!");

        try
        {
            await guild.CreateApplicationCommandAsync(guildCommand.Build());
        }
        catch 
        {
            LogService.Error("Error registering Discord Bot *TEST* slash commands");
            return; 
        }

        LogService.Log("Discord Bot *TEST* slash commands have been registered");
    }
}
