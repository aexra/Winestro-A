namespace Winestro_A.Structures;
public struct ConsoleCommandResult
{
    public bool Success;
    public string OutMessage;
    public Enums.ConsoleMessageTypes Type;

    public ConsoleCommandResult(string msg, bool ok = true, Enums.ConsoleMessageTypes type = Enums.ConsoleMessageTypes.Ok)
    {
        OutMessage = msg;
        Success = ok;
        Type = ok ? type : Enums.ConsoleMessageTypes.Fail;
    }
}
