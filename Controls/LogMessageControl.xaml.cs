using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Winestro_A.Enums;
using Winestro_A.Structures;

namespace Winestro_A.Controls;

public sealed partial class LogMessageControl : UserControl
{
    public LogMessage MSG = new()
    {
        Type = LogMessageTypes.Info,
        Text = string.Empty,
        Time = TimeSpan.Zero
    };

    public LogMessageTypes Type { get => MSG.Type; set => MSG.Type = value; }
    public string Text { get => MSG.Text; set => MSG.Text = value; }
    public TimeSpan Time { get => MSG.Time; set => MSG.Time = value; }

    public string? IconSource;

    public LogMessageControl()
    {
        switch (Type)
        {
            case LogMessageTypes.Info:
                IconSource = "ms-appx:///Assets/LogMessageInfoIcon.png";
                break;
            case LogMessageTypes.Warning:
                IconSource = "ms-appx:///Assets/LogMessageWarningIcon.png";
                break;
            case LogMessageTypes.Error:
                IconSource = "ms-appx:///Assets/LogMessageErrorIcon.png";
                break;
        }
        this.InitializeComponent();
    }
}
