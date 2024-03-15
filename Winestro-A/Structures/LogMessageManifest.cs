using Winestro_A.Enums;

namespace Winestro_A.Structures;

public struct LogMessageManifest
{
    public LogSeverity Type;
    public LogMeta Meta;
    public string Text;
    public string Time;
    public ulong Id;
}
