using System;

namespace HW5.Utils
{
    public static class RandomExtension
    {
        public static DateTime NextDateTime(this Random rnd, DateTime dateTimeFrom, DateTime dateTimeTo)
        {
            int days = (dateTimeTo - dateTimeFrom).Days * 60 * 60 * 24;
            int hours = (dateTimeTo - dateTimeFrom).Hours * 60 * 60;
            int minutes = (dateTimeTo - dateTimeFrom).Minutes * 60;
            int seconds = (dateTimeTo - dateTimeFrom).Seconds;

            int range = days + hours + minutes + seconds;

            int NumberOfSecondsToAdd = rnd.Next(range);
            return dateTimeFrom.AddSeconds(NumberOfSecondsToAdd);
        }
    }
}