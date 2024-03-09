using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Winestro_A.Structures;

public struct ConsoleCommandContext
{
    public List<string> Args;
    public Dictionary<string, string> Kwargs;
}
