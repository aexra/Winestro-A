using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Winestro_A.Services;

namespace Winestro_A.Discord;

public partial class DiscordBotService
{
    public static async Task RegisterSlashCommands()
    {


        LogService.Log("Discord Bot slash commands have been registered");
    }
}
