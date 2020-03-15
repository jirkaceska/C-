using System;
using System.Globalization;
using System.Linq;
using System.Text;

namespace MenuScrapper
{
    /// <summary>
    /// This class represents menu of the one day in week.
    /// This class is immutable.
    /// </summary>
    public class DayMenu
    {
        public DateTime Day { get; private set; }
        public string Soup { get; private set; }
        public Food[] Foods { get; private set; }

        /// <summary>
        /// Initializes new instance of DayMenu class
        /// </summary>
        /// <param name="date">Menu date</param>
        /// <param name="soup">Soup in menu</param>
        /// <param name="foods">Array of foods in menu</param>
        public DayMenu(DateTime date, string soup, Food[] foods)
        {
            Day = date;
            Soup = soup;
            Foods = new Food[foods.Length];
            foods.CopyTo(Foods, 0);
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine(Day.ToString("dd. MM. yyyy - dddd"));
            builder.Append("Polévka: ");
            builder.AppendLine(Soup ?? "-");
            for (int i = 0; i < Foods.Length; i++)
            {
                builder.AppendLine((i + 1) + ") " + Foods[i]);
            }
            builder.AppendLine();
            return builder.ToString();
        }

        /// <summary>
        /// Check if any food description contains substring.
        /// This method ignore cases (Copied from https://stackoverflow.com/questions/444798/case-insensitive-containsstring).
        /// </summary>
        /// <param name="str">Substring to check.</param>
        /// <returns>True if any food contains substring str.</returns>
        public bool Contains(string str) => Foods.Any(
            (food) => Utils.cultureInfo.CompareInfo.IndexOf(food.Description, str, CompareOptions.IgnoreCase) >= 0
        );
    }
}