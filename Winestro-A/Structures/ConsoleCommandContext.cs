using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Winestro_A.Structures;

public struct ConsoleCommandContext
{
    public string Name { get; private set; }
    public List<string> Args { get; private set; }
    public Dictionary<string, string> Kwargs { get; private set; }

    public ConsoleCommandContext(string name, List<string> args, Dictionary<string, string> kwargs)
    {
        Name = name;
        Args = args;
        Kwargs = kwargs;
    }
}
