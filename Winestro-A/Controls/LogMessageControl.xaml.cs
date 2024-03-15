using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Winestro_A.Enums;
using Winestro_A.Structures;

namespace Winestro_A.Controls;

public sealed partial class LogMessageControl : UserControl
{
    public LogSeverity Type { get; set; }
    public LogMeta Meta { get; set; } = LogMeta.Default;
    public string Text { get; set; }
    public string Time { get; set; }
    public ulong Id;

    public string IconSource 
    {
        get {
            switch (Type)
            {
                case LogSeverity.Warning:
                    return "ms-appx:///Assets/LogMessageWarningIcon.png";
                case LogSeverity.Error:
                    return "ms-appx:///Assets/LogMessageErrorIcon.png";
                default:
                    return "ms-appx:///Assets/LogMessageInfoIcon.png";
            }
        }
        set => IconSource = value; 
    }

    public LogMessageControl()
    {
        this.InitializeComponent();
    }
}
