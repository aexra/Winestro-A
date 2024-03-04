using Winestro_A.Enums;

namespace Winestro_A.Structures;

public struct LogMessage
{
    public LogMessageTypes Type;
    public string Text;
    public TimeSpan Time;
}
