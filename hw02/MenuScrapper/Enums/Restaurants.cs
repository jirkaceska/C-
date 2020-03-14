using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MenuScrapper.Enums
{
    /// <summary>
    /// Restaurants enum as bit mask. Used in filters.
    /// If you want add restaurant to scrapper, dont forget to add it to enum.
    /// </summary>
    [Flags]
    public enum Restaurants
    {
        UDrevaka = 1,
        AlCapone = 2,
        PlzenskyDvur = 4,
        //StopkovaPivnice = 8
    }
}
