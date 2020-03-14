using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MenuScrapper.Enums
{
    /// <summary>
    /// Option to select from main menu
    /// </summary>
    public enum Options
    {
        None,
        TodayAllRestaurants,
        TodaySelectedRestaurant,
        SelectedDayAllRestaurants,
        SelectedDaySelectedRestaurant,
        WeekAllRestaurants,
        Search,
        Quit
    }
}
