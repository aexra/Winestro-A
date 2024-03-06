using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Winestro_A.Attributes;

[AttributeUsage(AttributeTargets.Method)]
class ICCommandAttribute : Attribute
{
    public string Name { get; }
    public int nArgs { get; set; } = 0;
    public string[]? KwargsKeys { get; set; } = null;
    public string[]? Aliases { get; set; } = null;

    public ICCommandAttribute(string name)
    {
        Name = name;
    }
}
