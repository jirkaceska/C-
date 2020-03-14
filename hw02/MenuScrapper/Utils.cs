using HtmlAgilityPack;
using MenuScrapper.Enums;
using System;
using System.Globalization;
using System.Text;

namespace MenuScrapper
{
    public static class Utils
    {
        public static CultureInfo cultureInfo = new CultureInfo("cs-CZ", false);

        /// <summary>
        /// Use this method to get HtmlDocument by url (see Constants.cs).
        /// HtmlAgilityPack is required.
        /// </summary>
        /// <param name="url">Url of website with menu.</param>
        /// <returns>Loaded HTML document.</returns>
        /// <exception cref="System.Net.WebException">Web cannot be loaded - check your internet connection.</exception>
        public static HtmlDocument GetHtmlDoc(string url)
        {
            var web = new HtmlWeb();
            return web.Load(url);
        }


        /// <summary>
        /// Parse date string to DateTime object.
        /// E. g. X. Y. to thisYear-monthY-dayX.
        /// </summary>
        /// <param name="dateStr">String to parse</param>
        /// <returns>Parsed DateTime.</returns>
        public static DateTime ParseDateTime(string dateStr)
        {
            int year = DateTime.Now.Year;
            string[] tokens = dateStr.Split('.');
            int month = Convert.ToInt32(tokens[1].Trim());
            int day = Convert.ToInt32(tokens[0].Trim());
            return new DateTime(year, month, day);
        }


        /// <summary>
        /// Parse price string to int. Price is ended by splitter parameter
        /// E.g. 42,- to 42.
        /// </summary>
        /// <param name="priceStr">String to parse</param>
        /// <param name="splitter">Character on end of price - default is ','.</param>
        /// <returns>Parsed int.</returns>
        public static int ParsePrice(string priceStr, char splitter = ',') => Convert.ToInt32(priceStr.Split(splitter)[0]);


        /// <summary>
        /// Break long string into lines.
        /// String is aligned to left. Added trailing dots.
        /// If description is too long to fit on one line, it breaks in more lines. Every nexts is intended by padding left spaces (because of line numbers)
        /// </summary>
        /// <param name="str">String to format.</param>
        /// <param name="rowLength">Length of row.</param>
        /// <param name="paddingLeft">Indentation of lines.</param>
        /// <returns>Formatted string.</returns>
        public static string BreakStringIntoLines(string str, int rowLength, int paddingLeft = 3)
        {
            StringBuilder builder = new StringBuilder();
            int start = 0;
            while (start + rowLength < str.Length)
            {
                builder.AppendLine(str.Substring(start, rowLength));
                builder.Append("".PadLeft(paddingLeft));
                start += rowLength;
            }
            builder.Append(str.Substring(start, str.Length - start).PadRight(rowLength, '.'));

            return builder.ToString();
        }

        /// <summary>
        /// Get number of digits in number.
        /// </summary>
        /// <param name="x">Number to check.</param>
        /// <returns>Number of digits.</returns>
        public static int GetIntLength(int x) => (int)Math.Log10(x);

        /// <summary>
        /// Remove leading number in string.
        /// </summary>
        /// <param name="str">String to edit.</param>
        /// <param name="offsetAfterFirstDigit">How many next characters to also delete.</param>
        /// <returns>Formatted string.</returns>
        public static string RemoveLeadingNumbers(string str, int offsetAfterFirstDigit = 3)
        {
            int index = str.IndexOfAny("0123456789".ToCharArray()) + offsetAfterFirstDigit;
            return str.Substring(index, str.Length - index);
        }

        /// <summary>
        /// Extension method to extract FlagDayOfWeek from DateTime object.
        /// </summary>
        /// <param name="day">Date time</param>
        /// <returns>Flag day of week.</returns>
        public static FlagDayOfWeek FlagDayOfWeek(this DateTime day)
        {
            return (FlagDayOfWeek)(1 << (int)day.DayOfWeek);
        }
    }
}