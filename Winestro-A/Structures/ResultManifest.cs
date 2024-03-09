namespace Winestro_A.Structures;

public readonly struct ResultManifest
{
    public readonly bool Success { get; }
    public readonly string Message { get; }

    public ResultManifest(bool success, string message = "")
    {
        Success = success;
        Message = message;
    }
}
