using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Interactions;
using Discord.WebSocket;
using Winestro_A.Services;

namespace Winestro_A.Discord;

public class SlashTestModule : InteractionModuleBase<SocketInteractionContext>
{
    public override void OnModuleBuilding(InteractionService commandService, ModuleInfo module)
    {
        base.OnModuleBuilding(commandService, module);

        LogService.Log("Slash test commands module loaded successfully");
    }

    [SlashCommand("test", "Test slash command")]
    public async Task Test()
    {
        await RespondAsync("Hello, world!");
    }
}
