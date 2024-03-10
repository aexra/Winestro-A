using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Winestro_A.Services;

namespace Winestro_A.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public class ICCommandAttribute : Attribute
{
    public string Name { get; }
    public int RequiredArgs { get; set; } = 0;
    public string[]? KwargsKeys { get; set; } = null;

    public ICCommandAttribute(string name)
    {
        Name = name;
    }

    public bool IsNameEqual(string promt)
    {
        return promt.ToLower() == Name.ToLower();
    }
}
