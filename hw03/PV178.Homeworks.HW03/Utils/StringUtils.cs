using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PV178.Homeworks.HW03.Utils
{
    internal static class StringUtils
    {

        /// <summary>
        /// Get file name from file path.
        /// </summary>
        /// <param name="filePath">File path to parse.</param>
        /// <returns>Parsed file name.</returns>
        public static string ParseFileName(string filePath)
        {
            return filePath.Split(Path.DirectorySeparatorChar).Last().Split('.')[0];
        }

        private static readonly List<Func<int, int, bool>> conditions = new List<Func<int, int, bool>>
        {
            (x, y) => (x % 3 == 0) && (y % 5 != 0),
            (x, y) => (x % 3 != 0) && (y % 5 == 0),
            (x, y) => (x + y) % 7 == 0
        };

        /// <summary>
        /// Check if string is valid code.
        /// </summary>
        /// <param name="code">String to check.</param>
        /// <returns>True if code is valid.</returns>
        public static bool Evaluate(this string code)
        {
            var tokens = code.Split('I');
            if (tokens.Length == 2
                && Int32.TryParse(tokens[0], out int x)
                && Int32.TryParse(tokens[1], out int y)
                && 10 <= x && x < 100
                && 10 <= y && y < 100
                )
            {
                return conditions.Any(condition => condition(x, y));
            }
            return false;
        }
    }
}