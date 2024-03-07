using Winestro_A.Enums;

namespace Winestro_A.Structures;

public struct LogMessageManifest
{
    public LogMessageTypes Type;
    public LogMessageMetaTypes Meta;
    public string Text;
    public string Time;
    public ulong Id;
}
