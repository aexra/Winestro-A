using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Winestro_A.Structures;
public struct ConsoleCommand
{
    public string Name;
    public List<string> Args;
    public Dictionary<string, string> Kwargs;
}
