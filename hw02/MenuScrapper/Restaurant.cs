using System.Collections;
using System.Collections.Generic;

namespace MenuScrapper
{
    /// <summary>
    /// This class represents restaurant menu loaded from their websites.
    /// This class is immutable.
    /// It implements enumerable interface.
    /// </summary>
    public class Restaurant : IEnumerable<DayMenu>
    {
        public string Name { get; private set; }
        private readonly List<DayMenu> dayMenus;

        public Restaurant(string name, DayMenu[] menu)
        {
            Name = name;
            dayMenus = new List<DayMenu>(menu);
        }

        public IEnumerator<DayMenu> GetEnumerator()
        {
            return dayMenus.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override string ToString()
        {
            return Name;
        }
    }
}