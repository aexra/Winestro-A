using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Winestro_A.Controls;

namespace Winestro_A.Services;

public class ConfigService
{
    public static ObservableCollection<KeyValuePairEditableR> EditableControls { get; private set; } = new();

    public static void Init()
    {
        // TODO: Load JSON/conf and create KeyValuePairEditableRs for each line
    }
}
