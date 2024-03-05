using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Winestro_A.Enums;
using Winestro_A.Structures;

namespace Winestro_A.Controls;

public sealed partial class LogMessageControl : UserControl
{
    public LogMessageTypes Type { get; set; }
    public LogMessageMetaTypes Meta { get; set; }
    public string Text { get; set; }
    public string Time { get; set; }

    public string IconSource;

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
