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
        return DateTime.Now.TimeOfDay.ToString()[..8];
    }
    public static TimeSpan Now => DateTime.Now.TimeOfDay;
}
