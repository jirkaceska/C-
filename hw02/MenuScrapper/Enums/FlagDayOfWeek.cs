using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MenuScrapper.Enums
{
    /// <summary>
    /// DayOfWeek as bit mask. Used in filters.
    /// </summary>
    [Flags]
    public enum FlagDayOfWeek
    {
        None = 0,
        Sunday = 1,
        Monday = 2,
        Tuesday = 4,
        Wednesday = 8,
        Thursday = 16,
        Friday = 32,
        Saturday = 64,
    }
}
