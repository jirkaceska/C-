using System;

namespace MenuScrapper.Exceptions
{
    public class WeekendEmptyException : Exception
    {
        public WeekendEmptyException()
        {
        }

        public WeekendEmptyException(string message)
            : base(message)
        {
        }

        public WeekendEmptyException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}