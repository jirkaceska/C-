using System;

namespace MenuScrapper
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            MenuHandler handler = new MenuHandler();

            while (!handler.IsFinished)
            {
                string optionString = Console.ReadLine();
                handler.ParseOption(optionString);
            }
        }
    }
}