using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Winestro_A.Enums;
using Winestro_A.Structures;

namespace Winestro_A.Controls;

public sealed partial class LogMessageControl : UserControl
{
    public LogMessageTypes Type { get; set; }
    public LogMessageMetaTypes Meta { get; set; } = LogMessageMetaTypes.Default;
    public string Text { get; set; }
    public string Time { get; set; }

    public string IconSource 
    { 
        get {
            switch (Type)
            {
                case LogMessageTypes.Warning:
                    return "ms-appx:///Assets/LogMessageWarningIcon.png";
                case LogMessageTypes.Error:
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
