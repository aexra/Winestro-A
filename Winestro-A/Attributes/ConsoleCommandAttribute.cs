﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Winestro_A.Services;

namespace Winestro_A.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public class ConsoleCommandAttribute : Attribute
{
    public string Name { get; }
    public int RequiredArgs { get; set; } = 0;
    public string[]? KwargsKeys { get; set; } = null;
    public string Description { get; set; }

    public ConsoleCommandAttribute(string name)
    {
        Name = name;
    }

    public bool IsNameEqual(string promt)
    {
        return promt.ToLower() == Name.ToLower();
    }
}
