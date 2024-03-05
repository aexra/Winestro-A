using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Winestro_A.Structures;
public struct ConsoleCommandTemplate
{
    public string Name;
    public int nArgs;
    public String[] KwargsKeys;
    public Func<List<string>, Dictionary<string, string>, bool> Function;
}
