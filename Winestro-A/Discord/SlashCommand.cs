using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Windows.Media.Protection.PlayReady;
using Winestro_A.Services;

namespace Winestro_A.Discord;

public class SlashCommand
{
    public string Name { get; set; }
    public string Description { get; set; }
    public bool IsGlobal { get; set; }

    public Func<SlashCommandBuilder, SlashCommandBuilder>? PropertiesBuilder { get; set; }

    private Func<SocketSlashCommand, Task> Handler { get; set; }

    public SlashCommand(string name, string description, Func<SocketSlashCommand, Task> handler, bool isGlobal = false)
    {
        Name = name;
        Description = description;
        Handler = handler;
        IsGlobal = isGlobal;
    }

    public async Task Register(DiscordSocketClient client)
    {
        var command = new SlashCommandBuilder()
            .WithName(Name)
            .WithDescription(Description);

        if (PropertiesBuilder != null)
        {
            command = PropertiesBuilder(command);
        }

        if (IsGlobal)
        {
            await client.CreateGlobalApplicationCommandAsync(command.Build());
        }
        else
        {
            var guild = client.GetGuild(ulong.Parse(ConfigService.Get("DiscordTestGuildID").ToString()));
            await guild.CreateApplicationCommandAsync(command.Build());
        }
    }
    public async Task Handle(SocketSlashCommand command)
    {
        await Handler(command);
    }
}
