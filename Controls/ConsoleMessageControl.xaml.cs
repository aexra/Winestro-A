using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Winestro_A.Enums;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Winestro_A.Controls;

public sealed partial class ConsoleMessageControl : UserControl
{
    public ConsoleMessageTypes Type { get; set; }
    public string Text { get; set; }
    public string IconSource
    {
        get {
            switch (Type)
            {
                case ConsoleMessageTypes.Ok:
                    return "ms-appx:///Assets/LogMessageOkIcon.png";
                case ConsoleMessageTypes.Fail:
                    return "ms-appx:///Assets/LogMessageErrorIcon.png";
                case ConsoleMessageTypes.Command:
                    return "ms-appx:///Assets/LogMessageCommandIcon.png";
                default:
                    return "ms-appx:///Assets/LogMessageInfoIcon.png";
            }
        }
        set => IconSource = value;
    }

    public ConsoleMessageControl()
    {
        this.InitializeComponent();
    }
}
