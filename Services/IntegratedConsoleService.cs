using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Controls;

namespace Winestro_A.Services;
public class IntegratedConsoleService
{
    public static ObservableCollection<TextBlock> ConsoleHistory { get; private set; } = new();
}
