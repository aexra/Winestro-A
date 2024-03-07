using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Winestro_A.Services;

public class DiscordBotService
{


    public static async Task Run()
    {

    }

    private static Task Log(LogMessage msg)
    {
        switch (msg.Severity)
        {
            case LogSeverity.Error:
                LogService.Error(msg.Message);
                break;
            case LogSeverity.Warning:
                LogService.Warning(msg.Message);
                break;
            case LogSeverity.Info:
                LogService.Log(msg.Message);
                break;
            case LogSeverity.Critical:
                LogService.Error(msg.Message);
                break;
            default:
                LogService.Log(msg.Message, Enums.LogMessageMetaTypes.Debug);
                break;
        }
        return Task.CompletedTask;
    }
}
