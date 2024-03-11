using Discord.Interactions;
using Discord.WebSocket;
using Winestro_A.Services;

namespace Winestro_A.Discord;

public class DiscordSlashCommandsModule : InteractionModuleBase<SocketInteractionContext<SocketMessageComponent>>
{
    public override void OnModuleBuilding(InteractionService commandService, ModuleInfo module)
    {
        base.OnModuleBuilding(commandService, module);

        LogService.Log("Slash commands module successfully loaded");
    }

    [SlashCommand("echo", "Echo and input")]
    public async Task Echo(string input)
    {
        await RespondAsync(input);
    }
}
