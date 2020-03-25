using System;

namespace PV178.Homeworks.HW03.Utils
{
    /// <summary>
    /// Class for writing into console.
    /// </summary>
    public class Displayer
    {
        /// <summary>
        /// Displays the actual state of game.
        /// </summary>
        /// <param name="text">text of track</param>
        /// <param name="position">actual position</param>
        public void ActualDisplay(string text, int position)
        {
            Console.Clear();
            for (var i = position; i >= 0; i--)
            {
                if (position - i > 12) break;

                int spaceCount;
                if (position >= 6)
                {
                    if (i == position - 6)
                    {
                        spaceCount = GetSpaceCount(text[i]);
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("\t~" + new string(' ', spaceCount) + $"{text[i]}" +
                            new string(' ', 6 - spaceCount) + "~");
                        Console.ResetColor();
                        continue;
                    }
                }
                if (i >= text.Length)
                {
                    Console.WriteLine("\t ");
                    continue;
                }
                spaceCount = GetSpaceCount(text[i]);
                Console.WriteLine("\t " + new string(' ', spaceCount) + $"{text[i]}"
                    + new string(' ', 6 - spaceCount));
            }
            if (position < 6)
            {
                for (var i = position; i < 5; i++)
                {
                    Console.WriteLine(" ");
                }
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\t~       ~");
                Console.ResetColor();
            }
        }

        /// <summary>
        /// Returns number of spaces for better formating of key.
        /// </summary>
        /// <param name="key">key to format</param>
        /// <returns></returns>
        private int GetSpaceCount(char key)
        {
            switch (key)
            {
                case 'a':
                    return 0;
                case 's':
                    return 1;
                case 'd':
                    return 2;
                case 'f':
                    return 3;
                case 'g':
                    return 4;
                case 'h':
                    return 5;
                case 'j':
                    return 6;
                default:
                    return 6;
            }
        }
    }
}
