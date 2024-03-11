using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;

namespace Winestro_A.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public class SlashCommandAttribute : Attribute
{
    public string Name { get; }
    public string Description { get; }

    public bool IsGlobal { get; set; } = false;

    public SlashCommandAttribute(string name, string description)
    {
        Name = name;
        Description = description;
    }
}
