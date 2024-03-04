using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Winestro_A.Helpers;

public class TimeHelper
{
    public static string NowS()
    {
        var s = string.Empty;
        TimeSpan now = DateTime.Now.TimeOfDay;
        s += now.Hours.ToString() + ":" + now.Minutes.ToString() + ":" + now.Seconds.ToString();
        return s;
    }
    public static TimeSpan Now => DateTime.Now.TimeOfDay;
}
