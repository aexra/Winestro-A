using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Winestro_A.Services;

namespace Winestro_A.Attributes;

[AttributeUsage(AttributeTargets.Method)]
class ICCommandAttribute : Attribute
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
        var splitted_promt = promt.ToLower().Split(' ');
        var splitted_name = Name.ToLower().Split(' ');

        if (splitted_name.Length != splitted_promt.Length)
        {
            return false;
        }

        for (var i = 0; i < splitted_name.Length; i++)
        {
            var name_part = splitted_name[i];
            var promt_part = splitted_promt[i];

            if (name_part != promt_part)
            {
                return false;
            }
        }

        return true;
    }
}
